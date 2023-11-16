using Domain.Dto.Steam;

namespace Interface.Client;

public interface ISteamClient
{
    Task<SteamPlayerDto> GetSteamPlayerSummary(string accessToken);
}
