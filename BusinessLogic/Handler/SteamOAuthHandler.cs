using BusinessLogic.Database;
using Domain;
using Domain.Configuration;
using Domain.Entity;
using Interface.Client;
using Interface.Handler;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Handler;

public class SteamOAuthHandler : ISteamOAuthHandler
{
    private readonly ILogger<SteamOAuthHandler> logger;
    private readonly IOptions<SteamOAuthOptions> steamOAuthOptions;
    private readonly ApplicationContext context;
    private readonly ISteamClient steamClient;

    public SteamOAuthHandler(
        ILogger<SteamOAuthHandler> logger,
        IOptions<SteamOAuthOptions> steamOAuthOptions,
        ApplicationContext context,
        ISteamClient steamClient)
    {
        this.logger = logger;
        this.steamOAuthOptions = steamOAuthOptions;
        this.context = context;
        this.steamClient = steamClient;
    }

    public async Task<IResult> SteamLogin()
    {
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
        this.logger.LogInformation("{recordId}: User is being redirected to steam for oAuth login", record.Id);

        return this.SteamOAuthRedirect(record.Id);
    }

    public async Task<IResult> SteamLoginSuccess(
        HttpContext httpContext,
        Guid oAuthRecordId,
        string tokenType,
        string accessToken)
    {
        var record = await this.context.OAuthRecords
            .FindAsync(oAuthRecordId);
        if (record is null)
        {
            return Results.NotFound();
        }

        try
        {
            record.ReturnedFromSteam = DateTime.UtcNow;

            var playerData = await this.steamClient.GetSteamPlayerSummary(accessToken);
            record.SteamId = playerData.SteamId;
            record.AccessToken = accessToken;
            
            await this.context.SaveChangesAsync();
            this.logger.LogInformation("{recordId}: User successfully completed oAuth login with steamId: {steamId}", record.Id, playerData.SteamId);

            return this.SteamOAuthSuccessRedirect(httpContext, record.Id);
        }
        catch (Exception e)
        {
            this.logger.LogCritical("An exception was thrown handling OAuthRecord {OAuthRecordId} -> {e}", record.Id, e);
            return Results.StatusCode(500);
        }
    }

    public async Task<IResult> SteamLoginFailure(
        Guid oAuthRecordId,
        string error)
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

        return Results.Unauthorized();
    }

    public string ParseQueryParameters(string endpoint, Dictionary<string, string> parameters)
    {
        var queryString = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var baseUri = new Uri(endpoint);
        var uri = new Uri(baseUri, $"?{queryString}");
        return uri.AbsoluteUri;
    }

    private IResult SteamOAuthRedirect(Guid recordId)
    {
        var clientId = this.steamOAuthOptions.Value.ClientId;
        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new NullReferenceException("ClientId is not defined");
        }

        var endpoint = this.steamOAuthOptions.Value.OAuthEndpoint;
        var queryParams = new Dictionary<string, string>
        {
            { "response_type", "token" },
            { "client_id", clientId },
            { "state", recordId.ToString() },
        };
        
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return Results.RedirectToRoute(GptApiConstants.DeveloperIdpName, queryParams);
        }
        
        return Results.Redirect(this.ParseQueryParameters(endpoint, queryParams));
    }

    private IResult SteamOAuthSuccessRedirect(HttpContext httpContext, Guid oAuthRecordId)
    {
        // Define a cookie
        var cookieOptions = new CookieOptions
        {
            Path = "/",
            HttpOnly = true,
            Secure = true,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
        };

        httpContext.Response.Cookies.Append("Credentials", oAuthRecordId.ToString(), cookieOptions);
        
        var endpoint = this.steamOAuthOptions.Value.OAuthEndpoint;
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return Results.Redirect($"{GptApiConstants.DeveloperFrontendUrl}?login=true");
        }

        return Results.RedirectToRoute("/?login=true");
    }
}
