using Domain.Dto.Steam;

namespace Interface.Client;

public interface ISteamClient
{
    Task<string> GetSteamId(string accessToken);
    
    Task<SteamPlayerDto> GetSteamPlayerSummary(string steamId);
}
