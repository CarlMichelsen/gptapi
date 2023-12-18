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

namespace BusinessLogic.Handler.OAuth.Discord;

public class DiscordOAuthLoginHandler : IOAuthLoginHandler
{
    private readonly ILogger<DiscordOAuthLoginHandler> logger;
    private readonly ApplicationContext applicationContext;
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly IEndpointUrlProvider endpointUrlProvider;

    public DiscordOAuthLoginHandler(
        ILogger<DiscordOAuthLoginHandler> logger,
        ApplicationContext applicationContext,
        IOptions<DiscordOptions> discordOptions,
        IEndpointUrlProvider endpointUrlProvider)
    {
        this.logger = logger;
        this.applicationContext = applicationContext;
        this.discordOptions = discordOptions;
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
                nameof(DiscordOAuthLoginHandler),
                e);
            
            return Results.StatusCode(500);
        }
    }

    private string GenerateGithubOAuthUrl(OAuthRecord oAuthRecord)
    {
        var redirectUri = this.endpointUrlProvider
            .GetEndpointUrlFromEndpointName(GptApiConstants.DiscordLoginRedirectEndPointName);

        var parameters = new Dictionary<string, string>
        {
            { "response_type", "code" },
            { "client_id", this.discordOptions.Value.ClientId },
            { "redirect_uri", redirectUri },
            { "scope", "identify,email" },
            { "state", oAuthRecord.Id.ToString() },
        };

        this.logger.LogCritical("discord redirect-url: \"{redirectUri}\"", redirectUri);

        var queryString = this.endpointUrlProvider.GenerateQueryParamsToAppend(parameters);
        var baseUri = new Uri(this.discordOptions.Value.OAuthEndpoint);
        return baseUri + queryString;
    }

    private async Task<OAuthRecord> RegisterOAuthRecord(OAuthRecordId id)
    {
        var oAuthRecord = new OAuthRecord
        {
            Id = id,
            AuthenticationMethod = AuthMethods.Discord,
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
