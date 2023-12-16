using Domain.Entity;
using Domain.OAuth;
using Interface.Client;

namespace BusinessLogic.Client;

public class SteamOAuthClient : IOAuthClient
{
    public Task<string> GetOAuthId(string accessToken)
    {
        // https://partner.steamgames.com/doc/webapi_overview/oauth#RetrieveSteamID
        // https://api.steampowered.com/ISteamUserOAuth/GetTokenDetails/v1/?access_token=token
        throw new NotImplementedException();
    }

    public Task<IOAuthUserDataConvertible> GetOAuthUserData(OAuthRecord oAuthRecord)
    {
        // https://partner.steamgames.com/doc/webapi/ISteamUser#GetPlayerSummaries
        // https://partner.steam-api.com/ISteamUser/GetPlayerSummaries/v2/
        throw new NotImplementedException();
    }
}
