using Domain.Entity;
using Domain.OAuth;
using Interface.Client;

namespace BusinessLogic.Client;

public class DiscordOAuthClient : IOAuthClient
{
    public Task<string> GetOAuthId(string accessToken)
    {
        throw new NotImplementedException();
    }

    public Task<IOAuthUserDataConvertible> GetOAuthUserData(OAuthRecord oAuthRecord)
    {
        throw new NotImplementedException();
    }
}
