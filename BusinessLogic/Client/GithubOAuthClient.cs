using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain;
using Domain.Configuration;
using Domain.Dto.Github;
using Domain.Entity;
using Domain.Exception;
using Domain.OAuth;
using Interface.Client;
using Interface.Provider;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Client;

public class GithubOAuthClient : IOAuthClient
{
    private const string AccessTokenPath = "/login/oauth/access_token";
    private const string UserPath = "/user";

    private readonly ILogger<GithubOAuthClient> logger;
    private readonly HttpClient githubOAuthHttpClient;
    private readonly HttpClient githubApiHttpClient;
    private readonly IEndpointUrlProvider endpointUrlProvider;
    private readonly IOptions<GithubOAuthOptions> githubOAuthOptions;

    public GithubOAuthClient(
        ILogger<GithubOAuthClient> logger,
        IHttpClientFactory clientFactory,
        IEndpointUrlProvider endpointUrlProvider,
        IOptions<GithubOAuthOptions> githubOAuthOptions)
    {
        this.logger = logger;
        this.endpointUrlProvider = endpointUrlProvider;
        this.githubOAuthOptions = githubOAuthOptions;

        this.githubOAuthHttpClient = clientFactory.CreateClient(GptApiConstants.GithubHttpClient);
        this.githubOAuthHttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        this.githubApiHttpClient = clientFactory.CreateClient(GptApiConstants.GithubAPIHttpClient);
        this.githubApiHttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<CodeResponseDto> ExchangeTheCode(string code)
    {
        var redirectUrl = this.endpointUrlProvider
            .GetEndpointUrlFromEndpointName(GptApiConstants.GithubLoginRedirectEndPointName);

        var payload = new Dictionary<string, string>
        {
            { "client_id", this.githubOAuthOptions.Value.ClientId },
            { "client_secret", this.githubOAuthOptions.Value.ClientSecret },
            { "code", code },
            { "redirect_uri", redirectUrl },
        };
        
        var response = await this.githubOAuthHttpClient.PostAsync(AccessTokenPath, new FormUrlEncodedContent(payload));
        response.EnsureSuccessStatusCode();

        try
        {
            return await response.Content.ReadFromJsonAsync<CodeResponseDto>()
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
        var user = await this.GetUser(accessToken);
        return user.Id.ToString();
    }

    public async Task<IOAuthUserDataConvertible> GetOAuthUserData(OAuthRecord oAuthRecord)
    {
        var accessToken = oAuthRecord.AccessToken
            ?? throw new ClientException("No AccessToken for github GetOAuthUserData");

        return await this.GetUser(accessToken);
    }

    private async Task<GithubUser> GetUser(string accessToken)
    {
        HttpResponseMessage? response = default;
        try
        {
            this.githubApiHttpClient.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Bearer", accessToken);
            response = await this.githubApiHttpClient.GetAsync(UserPath);

            response?.EnsureSuccessStatusCode();

            return await response?.Content.ReadFromJsonAsync<GithubUser>()
                ?? throw new ClientException("Could not parse GithubUser response");
        }
        catch (HttpRequestException)
        {
            this.logger.LogCritical("No access to github User data");
            throw;
        }
        catch (Exception)
        {
            var jsonStr = await response?.Content.ReadAsStringAsync();
            this.logger.LogCritical("Probably failed to parse the following string into a GithubUser object:\n{json}", jsonStr);
            throw;
        }
    }
}
