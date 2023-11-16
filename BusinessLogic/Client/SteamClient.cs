using Domain.Dto.Steam;
using Interface.Client;

namespace BusinessLogic.Client;

public class SteamClient : ISteamClient
{
    public Task<SteamPlayerDto> GetSteamPlayerSummary(string accessToken)
    {
        throw new NotImplementedException();
    }
}
