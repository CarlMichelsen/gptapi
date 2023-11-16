using Domain.Dto.Steam;
using Interface.Client;

namespace BusinessLogic.Client;

public class SteamClient : ISteamClient
{
    public Task<string> GetSteamId(string accessToken)
    {
        // https://partner.steamgames.com/doc/webapi_overview/oauth#RetrieveSteamID
        // https://api.steampowered.com/ISteamUserOAuth/GetTokenDetails/v1/?access_token=token
        throw new NotImplementedException();
    }

    public Task<SteamPlayerDto> GetSteamPlayerSummary(string accessToken)
    {
        // https://partner.steamgames.com/doc/webapi/ISteamUser#GetPlayerSummaries
        // https://partner.steam-api.com/ISteamUser/GetPlayerSummaries/v2/
        throw new NotImplementedException();
    }
}
