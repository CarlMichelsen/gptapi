using Interface.Handler;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class SteamOAuthEndpoints
{
    public static RouteGroupBuilder MapSteamOAuthEndpoints(this RouteGroupBuilder group)
    {
        var oAuthGroup = group.MapGroup("/oauth");

        oAuthGroup.MapGet(
            "/SteamLogin",
            async ([FromServices] ISteamOAuthHandler steamOauthHandler) => await steamOauthHandler.SteamLogin())
            .WithName("SteamLogin");

        oAuthGroup.MapGet(
            "/SteamLoginSuccess",
            async (
                [FromServices] ISteamOAuthHandler steamOauthHandler,
                [FromQuery(Name = "access_token")] string accessToken,
                [FromQuery(Name = "token_type")] string tokenType,
                [FromQuery(Name = "state")] Guid oAuthRecordId) => 
            {
                return await steamOauthHandler.SteamLoginSuccess(oAuthRecordId, tokenType, accessToken);
            })
            .WithName("SteamLoginSuccess");

        oAuthGroup.MapGet(
            "/SteamLoginFailure",
            async (
                [FromServices] ISteamOAuthHandler steamOauthHandler,
                [FromQuery(Name = "error")] string error,
                [FromQuery(Name = "state")] Guid oAuthRecordId) => 
            {
                return await steamOauthHandler.SteamLoginFailure(oAuthRecordId, error);
            })
            .WithName("SteamLoginFailure");
        return group;
    }
}
