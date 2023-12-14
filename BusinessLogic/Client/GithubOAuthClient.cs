using Domain.OAuth;
using Interface.Client;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Client;

public class GithubOAuthClient : IOAuthClient
{
    private readonly ILogger<GithubOAuthClient> logger;
    private readonly HttpClient githubOAuthHttpClient;

    public GithubOAuthClient(
        ILogger<GithubOAuthClient> logger,
        HttpClient githubOAuthHttpClient)
    {
        this.logger = logger;
        this.githubOAuthHttpClient = githubOAuthHttpClient;
    }

    public Task<string> GetOAuthId(string accessToken)
    {
        // POST https://github.com/login/oauth/access_token
        throw new NotImplementedException();
    }

    public Task<IOAuthUserDataConvertible> GetOAuthUserData(string oAuthId, string? code = null)
    {
        throw new NotImplementedException();
    }
}
