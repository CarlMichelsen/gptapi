using Microsoft.AspNetCore.Http;

namespace Interface.Handler;

public interface ISteamOAuthHandler
{
    Task<IResult> SteamLogin();

    Task<IResult> SteamLoginSuccess(
        Guid oAuthRecordId,
        string tokenType,
        string accessToken);
    
    Task<IResult> SteamLoginFailure(
        Guid oAuthRecordId,
        string error);
}
