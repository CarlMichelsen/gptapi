using Database;
using Domain;
using Domain.Configuration;
using Domain.Entity;
using Domain.Entity.Id;
using Domain.Exception;
using Interface.Handler.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Handler.OAuth.Github;

public class GithubOAuthLoginHandler : IOAuthLoginHandler
{
    private readonly ILogger<GithubOAuthLoginHandler> logger;
    private readonly ApplicationContext applicationContext;
    private readonly IOptions<GithubOAuthOptions> githubOAuthOptions;
    private readonly LinkGenerator linkGenerator;
    private readonly IHttpContextAccessor httpContextAccessor;

    public GithubOAuthLoginHandler(
        ILogger<GithubOAuthLoginHandler> logger,
        ApplicationContext applicationContext,
        IOptions<GithubOAuthOptions> githubOAuthOptions,
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        this.logger = logger;
        this.applicationContext = applicationContext;
        this.githubOAuthOptions = githubOAuthOptions;
        this.linkGenerator = linkGenerator;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<IResult> Login()
    {
        try
        {
            var oAuthRecord = await this.RegisterOAuthRecord(new OAuthRecordId(Guid.NewGuid()));
            var url = this.GenerateGithubOAuthUrl(oAuthRecord);
            return Results.Redirect(url);
        }
        catch (Exception e)
        {
            this.logger.LogCritical(
                "Critical failure in {handlername} {exception}",
                nameof(GithubOAuthLoginHandler),
                e);
            
            return Results.StatusCode(500);
        }
    }

    private string GenerateGithubOAuthUrl(OAuthRecord oAuthRecord)
    {
        var httpContext = this.httpContextAccessor.HttpContext
            ?? throw new OAuthException("HttpContext not available");

        var redirectUri = this.linkGenerator.GetUriByName(httpContext, GptApiConstants.GithubLoginSuccessEndPointName)
            ?? throw new OAuthException("Failed to get development OAuth redirect url");

        var parameters = new Dictionary<string, string>
        {
            { "response_type", "token" },
            { "client_id", this.githubOAuthOptions.Value.ClientId },
            { "redirect_uri", redirectUri },
            { "scope", "user" },
            { "state", oAuthRecord.Id.ToString() },
            { "allow_signup", "false" },
        };

        var queryString = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var baseUri = new Uri(this.githubOAuthOptions.Value.OAuthEndpoint);
        return $"{baseUri}?{queryString}";
    }

    private async Task<OAuthRecord> RegisterOAuthRecord(OAuthRecordId id)
    {
        var oAuthRecord = new OAuthRecord
        {
            Id = id,
            AuthenticationMethod = AuthenticationMethod.Github,
            RedirectedToThirdParty = DateTime.UtcNow,
            ReturnedFromThirdParty = null,
            UserId = null,
            AccessToken = null,
            Error = null,
        };

        this.applicationContext.OAuthRecord.Add(oAuthRecord);
        await this.applicationContext.SaveChangesAsync();

        return oAuthRecord;
    }
}
