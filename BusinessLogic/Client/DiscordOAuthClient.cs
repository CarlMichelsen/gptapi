using System.Net.Http.Json;
using Domain;
using Domain.Configuration;
using Domain.Entity;
using Domain.OAuth;
using Interface.Client;
using Interface.Provider;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Client;

public class DiscordOAuthClient : IOAuthClient
{
    private const string AccessTokenPath = "/api/oauth2/token";

    private readonly ILogger<DiscordOAuthClient> logger;
    private readonly HttpClient httpClient;
    private readonly IEndpointUrlProvider endpointUrlProvider;
    private readonly IOptions<DiscordOptions> discordOptions;

    public DiscordOAuthClient(
        ILogger<DiscordOAuthClient> logger,
        HttpClient httpClient,
        IEndpointUrlProvider endpointUrlProvider,
        IOptions<DiscordOptions> discordOptions)
    {
        this.logger = logger;
        this.httpClient = httpClient;
        this.endpointUrlProvider = endpointUrlProvider;
        this.discordOptions = discordOptions;
    }

    public async Task<string> ExchangeTheCode(string code)
    {
        var redirectUrl = this.endpointUrlProvider
            .GetEndpointUrlFromEndpointName(GptApiConstants.DiscordLoginRedirectEndPointName);

        var payload = new Dictionary<string, string>
        {
            { "client_id", this.discordOptions.Value.ClientId },
            { "client_secret", this.discordOptions.Value.ClientSecret },
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", redirectUrl },
        };

        var content = new FormUrlEncodedContent(payload);

        var response = await this.httpClient.PostAsync(AccessTokenPath, content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        this.logger.LogCritical("ExchangeTheCodeResponse:\n{res}", responseContent);

        throw new NotImplementedException();
    }

    public Task<string> GetOAuthId(string accessToken)
    {
        throw new NotImplementedException();
    }

    public Task<IOAuthUserDataConvertible> GetOAuthUserData(OAuthRecord oAuthRecord)
    {
        throw new NotImplementedException();
    }
}
