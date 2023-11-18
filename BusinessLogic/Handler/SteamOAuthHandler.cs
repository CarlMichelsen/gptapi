using System.Security.Claims;
using BusinessLogic.Database;
using Domain;
using Domain.Configuration;
using Domain.Entity;
using Domain.Exception;
using Interface.Factory;
using Interface.Handler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Handler;

public class SteamOAuthHandler : ISteamOAuthHandler
{
    private readonly ILogger<SteamOAuthHandler> logger;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IOptions<SteamOAuthOptions> steamOAuthOptions;
    private readonly IOptions<ApplicationOptions> applicationOptions;
    private readonly ISteamClientFactory steamClientFactory;
    private readonly ApplicationContext context;

    public SteamOAuthHandler(
        ILogger<SteamOAuthHandler> logger,
        IHttpContextAccessor httpContextAccessor,
        IOptions<SteamOAuthOptions> steamOAuthOptions,
        IOptions<ApplicationOptions> applicationOptions,
        ISteamClientFactory steamClientFactory,
        ApplicationContext context)
    {
        this.logger = logger;
        this.httpContextAccessor = httpContextAccessor;
        this.steamOAuthOptions = steamOAuthOptions;
        this.applicationOptions = applicationOptions;
        this.steamClientFactory = steamClientFactory;
        this.context = context;
    }

    public static string ParseQueryParameters(string endpoint, Dictionary<string, string> parameters)
    {
        var queryString = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var baseUri = new Uri(endpoint);
        var uri = new Uri(baseUri, $"?{queryString}");
        return uri.AbsoluteUri;
    }

    public async Task<IResult> SteamLogin()
    {
        var error = this.InitialLoginErrors();
        if (error is not null)
        {
            throw new OAuthException(error);
        }

        var record = new OAuthRecord
        {
            Id = Guid.NewGuid(),
            RedirectedToSteam = DateTime.UtcNow,
            ReturnedFromSteam = null,
            SteamId = null,
            AccessToken = null,
            Error = null,
        };

        this.context.OAuthRecords.Add(record);
        await this.context.SaveChangesAsync();
        this.logger.LogInformation(
            "{recordId}: User is being redirected to steam for oAuth login",
            record.Id);

        var queryParams = new Dictionary<string, string>
        {
            { "response_type", "token" },
            { "client_id", this.steamOAuthOptions.Value.ClientId },
            { "state", record.Id.ToString() },
        };

        if (this.applicationOptions.Value.IsDevelopment)
        {
            return Results.RedirectToRoute(GptApiConstants.DeveloperIdpName, queryParams); 
        }

        var url = ParseQueryParameters(this.steamOAuthOptions.Value.OAuthEndpoint, queryParams);
        return Results.Redirect(url);
    }

    public async Task<IResult> SteamLoginFailure(Guid oAuthRecordId, string error)
    {
        var record = await this.context.OAuthRecords
            .FindAsync(oAuthRecordId);
        if (record is null)
        {
            return Results.NotFound();
        }

        record.Error = error;
        record.ReturnedFromSteam = DateTime.UtcNow;
        await this.context.SaveChangesAsync();
        this.logger.LogInformation("{recordId}: User failed oAuth login", record.Id);

        await this.SignOut();

        return Results.Unauthorized();
    }

    public async Task<IResult> SteamLoginSuccess(Guid oAuthRecordId, string tokenType, string accessToken)
    {
        var record = await this.context.OAuthRecords
            .FindAsync(oAuthRecordId);
        
        var error = this.LoginErrors(record);
        if (error is not null)
        {
            throw new OAuthException(error);
        }

        try
        {
            record!.ReturnedFromSteam = DateTime.UtcNow;

            var client = this.steamClientFactory.Create();
            var steamId = await client.GetSteamId(accessToken);

            record.SteamId = steamId;
            record.AccessToken = accessToken;

            await this.context.SaveChangesAsync();
            if (!record.IsCompleted())
            {
                throw new OAuthException("OAuth process should have completed by now.");
            }

            await this.AddCookieResponseHeader(record);
            this.logger.LogInformation(
                "{recordId}: User successfully completed oAuth login with steamId: {steamId}",
                record.Id,
                steamId);
            
            if (this.applicationOptions.Value.IsDevelopment)
            {
                return Results.Redirect(GptApiConstants.DeveloperFrontendUrl);
            }

            return Results.RedirectToRoute("/");
        }
        catch (Exception e)
        {
            this.logger.LogCritical(
                "An exception was thrown handling OAuthRecord {OAuthRecordId} -> {e}",
                record!.Id,
                e);
            return Results.StatusCode(500);
        }
    }

    public virtual async Task SignOut()
    {
        await this.httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public virtual async Task AddCookieResponseHeader(OAuthRecord record)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, record.Id.ToString()),
            new Claim("SteamId", record.SteamId!),
        };
        var claimsIdentity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties { };

        await this.httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }

    private string? InitialLoginErrors()
    {
        if (string.IsNullOrWhiteSpace(this.steamOAuthOptions.Value.ClientId))
        {
            return "Attempted to login without an OAuth ClientId";
        }

        return default;
    }

    private string? LoginErrors(OAuthRecord? record)
    {
        if (record is null)
        {
            return "Record was not found.";
        }

        if (record.IsCompleted())
        {
            return "This login process has already been completed.";
        }

        return default;
    }
}
