using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain;
using Domain.Configuration;
using Domain.Dto.Github;
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
        this.githubApiHttpClient = clientFactory.CreateClient(GptApiConstants.GithubAPIHttpClient);
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

        return await response.Content.ReadFromJsonAsync<CodeResponseDto>()
            ?? throw new ClientException("Could not parse code-exchange response");
    }
    
    public async Task<string> GetOAuthId(string accessToken)
    {
        var codeResponse = await this.ExchangeTheCode(accessToken);
        await this.GetThings(codeResponse.AccessToken);

        // POST https://github.com/login/oauth/access_token
        throw new NotImplementedException();
    }

    public Task<IOAuthUserDataConvertible> GetOAuthUserData(string oAuthId, string? code = null)
    {
        throw new NotImplementedException();
    }

    private async Task GetThings(string accessToken)
    {
        this.githubApiHttpClient.DefaultRequestHeaders.Authorization
            = new AuthenticationHeaderValue("Bearer", accessToken);
        
        var response = await this.githubApiHttpClient.GetAsync(UserPath);
        response.EnsureSuccessStatusCode();

        var resContent = await response.Content.ReadAsStringAsync();

        this.logger.LogCritical("githubuser: {resContent}", resContent);
    }
}
