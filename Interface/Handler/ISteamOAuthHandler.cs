using Microsoft.AspNetCore.Http;

namespace Interface.Handler;

public interface ISteamOAuthHandler
{
    Task<IResult> SteamLogin();

    Task<IResult> SteamLoginSuccess(
        HttpContext httpContext,
        Guid oAuthRecordId,
        string tokenType,
        string accessToken);
    
    Task<IResult> SteamLoginFailure(
        HttpContext httpContext,
        Guid oAuthRecordId,
        string error);
}
