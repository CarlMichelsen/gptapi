using Database;
using Domain;
using Domain.Configuration;
using Domain.Entity;
using Domain.Entity.Id;
using Interface.Handler.OAuth;
using Interface.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Handler.OAuth.Github;

public class GithubOAuthLoginHandler : IOAuthLoginHandler
{
    private readonly ILogger<GithubOAuthLoginHandler> logger;
    private readonly ApplicationContext applicationContext;
    private readonly IOptions<GithubOAuthOptions> githubOAuthOptions;
    private readonly IEndpointUrlProvider endpointUrlProvider;

    public GithubOAuthLoginHandler(
        ILogger<GithubOAuthLoginHandler> logger,
        ApplicationContext applicationContext,
        IOptions<GithubOAuthOptions> githubOAuthOptions,
        IEndpointUrlProvider endpointUrlProvider)
    {
        this.logger = logger;
        this.applicationContext = applicationContext;
        this.githubOAuthOptions = githubOAuthOptions;
        this.endpointUrlProvider = endpointUrlProvider;
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
        var redirectUri = this.endpointUrlProvider
            .GetEndpointUrlFromEndpointName(GptApiConstants.GithubLoginSuccessEndPointName);

        var parameters = new Dictionary<string, string>
        {
            { "response_type", "token" },
            { "client_id", this.githubOAuthOptions.Value.ClientId },
            { "redirect_uri", redirectUri },
            { "scope", "user" },
            { "state", oAuthRecord.Id.ToString() },
            { "allow_signup", "false" },
        };

        this.logger.LogCritical("github redirect-url: \"{redirectUri}\"", redirectUri);

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
