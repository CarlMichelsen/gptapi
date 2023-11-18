using Domain.Dto.Steam;
using Domain.Exception;
using Interface.Client;
using Interface.Provider;

namespace BusinessLogic.Client;

public class DevelopmentSteamClient : ISteamClient
{
    private readonly IDevelopmentIdentityProvider developmentIdentityProvider;

    public DevelopmentSteamClient(
        IDevelopmentIdentityProvider developmentIdentityProvider)
    {
        this.developmentIdentityProvider = developmentIdentityProvider;
    }

    public async Task<string> GetSteamId(string accessToken)
    {
        var devUsers = await this.developmentIdentityProvider
            .GetDevelopmentUsers();

        var user = devUsers.Players.FirstOrDefault(d => d.DevelopmentAccessToken == accessToken)
            ?? throw new ClientException("Failed to find development user that matches the accessToken");
        
        return user.SteamId;
    }

    public async Task<SteamPlayerDto> GetSteamPlayerSummary(string steamId)
    {
        var devUsers = await this.developmentIdentityProvider
            .GetDevelopmentUsers();
        
        return devUsers.Players.FirstOrDefault(d => d.SteamId == steamId)
            ?? throw new ClientException("Failed to find development user that matches the steamId");
    }
}
