using Interface.Handler;
using Microsoft.AspNetCore.Mvc;

namespace Api;

public static class SteamOAuthEndpoints
{
    public static RouteGroupBuilder MapSteamOAuthEndpoints(this RouteGroupBuilder group)
    {
        var oAuthGroup = group.MapGroup("/oauth");

        oAuthGroup.MapGet(
            "/SteamLogin",
            async ([FromServices] ISteamOAuthHandler steamOauthHandler) => await steamOauthHandler.SteamLogin())
            .WithName("Steam oAuth Login");

        oAuthGroup.MapGet(
            "/SteamLoginSuccess",
            async ([FromServices] ISteamOAuthHandler steamOauthHandler, [FromQuery(Name = "state")] Guid oAuthRecordId, [FromQuery(Name = "access_token")] string accessToken) => 
            {
                return await steamOauthHandler.SteamLoginSuccess(oAuthRecordId, accessToken);
            })
            .WithName("Steam oAuth Success Endpoint");

        oAuthGroup.MapGet(
            "/SteamLoginFailure",
            async ([FromServices] ISteamOAuthHandler steamOauthHandler, [FromQuery(Name = "state")] Guid oAuthRecordId, [FromQuery(Name = "error")] string error) => 
            {
                return await steamOauthHandler.SteamLoginFailure(oAuthRecordId, error);
            })
            .WithName("Steam oAuth Failure Endpoint");

        oAuthGroup.WithOpenApi();
        return group;
    }
}
