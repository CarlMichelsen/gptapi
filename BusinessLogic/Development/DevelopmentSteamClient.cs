using Domain.Dto.Steam;
using Interface.Client;
using Interface.Handler;

namespace BusinessLogic.Development;

public class DevelopmentSteamClient : ISteamClient
{
    private readonly IDevelopmentIdpHandler developmentIdpHandler;

    public DevelopmentSteamClient(
        IDevelopmentIdpHandler developmentIdpHandler)
    {
        this.developmentIdpHandler = developmentIdpHandler;
    }

    public async Task<SteamPlayerDto> GetSteamPlayerSummary(string accessToken)
    {
        var devUsers = await this.developmentIdpHandler
            .GetDevelopmentUsers();
        
        return devUsers.Players.FirstOrDefault(d => d.DevelopmentAccessToken == accessToken)
            ?? throw new Exception("Failed to find development user that matches the accessToken");
    }
}
