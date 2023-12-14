using Domain.OAuth;

namespace Interface.Client;

public interface IOAuthClient
{
    Task<string> GetOAuthId(string accessToken);

    Task<IOAuthUserDataConvertible> GetOAuthUserData(string oAuthId, string? code = null);
}
