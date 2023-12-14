using Domain.Exception;
using Domain.OAuth;
using Interface.Client;
using Interface.Provider;

namespace BusinessLogic.Client;

public class DevelopmentOAuthClient : IOAuthClient
{
    private readonly IDevelopmentIdentityProvider developmentIdentityProvider;

    public DevelopmentOAuthClient(
        IDevelopmentIdentityProvider developmentIdentityProvider)
    {
        this.developmentIdentityProvider = developmentIdentityProvider;
    }

    public async Task<string> GetOAuthId(string accessToken)
    {
        var devUsers = await this.developmentIdentityProvider
            .GetDevelopmentUsers();

        var user = devUsers.Players.FirstOrDefault(d => d.DevelopmentAccessToken == accessToken)
            ?? throw new ClientException("Failed to find development user that matches the accessToken");
        
        return user.SteamId;
    }

    public async Task<IOAuthUserDataConvertible> GetOAuthUserData(string oAuthId, string? code = null)
    {
        var devUsers = await this.developmentIdentityProvider
            .GetDevelopmentUsers();
        
        return devUsers.Players.FirstOrDefault(d => d.SteamId == oAuthId)
            ?? throw new ClientException("Failed to find development user that matches the steamId");
    }
}
