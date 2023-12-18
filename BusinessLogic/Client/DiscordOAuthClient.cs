using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain;
using Domain.Configuration;
using Domain.Dto.Discord;
using Domain.Entity;
using Domain.Exception;
using Domain.OAuth;
using Interface.Client;
using Interface.Provider;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Client;

public class DiscordOAuthClient : IOAuthClient
{
    private const string AccessTokenPath = "/api/oauth2/token";
    private const string UserPath = "/api/users/@me";

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

    public async Task<DiscordCodeResponse> ExchangeTheCode(string code)
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

        try
        {
            return await response.Content.ReadFromJsonAsync<DiscordCodeResponse>()
                ?? throw new ClientException("Could not parse code-exchange response");
        }
        catch (Exception)
        {
            var jsonStr = await response.Content.ReadAsStringAsync();
            this.logger.LogCritical("Failed to parse the following string into a CodeResponseDto object:\n{json}", jsonStr);
            throw;
        }
    }

    public async Task<string> GetOAuthId(string accessToken)
    {
        var discordUser = await this.GetUser(accessToken);
        return discordUser.Id;
    }

    public async Task<IOAuthUserDataConvertible> GetOAuthUserData(OAuthRecord oAuthRecord)
    {
        return await this.GetUser(oAuthRecord.AccessToken!);
    }

    private async Task<DiscordUser> GetUser(string accessToken)
    {
        this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await this.httpClient.GetAsync(UserPath);
        response.EnsureSuccessStatusCode();

        try
        {
            return await response.Content.ReadFromJsonAsync<DiscordUser>()
                ?? throw new ClientException("Could not parse code-exchange response");
        }
        catch (Exception)
        {
            var jsonStr = await response.Content.ReadAsStringAsync();
            this.logger.LogCritical("Failed to parse the following string into a CodeResponseDto object:\n{json}", jsonStr);
            throw;
        }
    }
}
