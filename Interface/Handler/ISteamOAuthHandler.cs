using Domain.Entity.Id;
using Microsoft.AspNetCore.Http;

namespace Interface.Handler;

public interface ISteamOAuthHandler
{
    Task<IResult> SteamLogin();

    Task<IResult> SteamLoginSuccess(
        OAuthRecordId oAuthRecordId,
        string tokenType,
        string accessToken);
    
    Task<IResult> SteamLoginFailure(
        OAuthRecordId oAuthRecordId,
        string error);
}
