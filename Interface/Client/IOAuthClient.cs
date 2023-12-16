using Domain.Entity;
using Domain.OAuth;

namespace Interface.Client;

public interface IOAuthClient
{
    Task<string> GetOAuthId(string accessToken);

    Task<IOAuthUserDataConvertible> GetOAuthUserData(OAuthRecord oAuthRecord);
}
