using Domain.Exception;
using Interface.Client;
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
            .WithName("SteamLogin");

        oAuthGroup.MapGet(
            "/SteamLoginSuccess",
            async (
                HttpContext httpContext,
                [FromServices] ISteamOAuthHandler steamOauthHandler,
                [FromQuery(Name = "access_token")] string accessToken,
                [FromQuery(Name = "token_type")] string tokenType,
                [FromQuery(Name = "state")] Guid oAuthRecordId) => 
            {
                return await steamOauthHandler.SteamLoginSuccess(httpContext, oAuthRecordId, tokenType, accessToken);
            })
            .WithName("SteamLoginSuccess");

        oAuthGroup.MapGet(
            "/SteamLoginFailure",
            async (
                HttpContext httpContext,
                [FromServices] ISteamOAuthHandler steamOauthHandler,
                [FromQuery(Name = "error")] string error,
                [FromQuery(Name = "state")] Guid oAuthRecordId) => 
            {
                return await steamOauthHandler.SteamLoginFailure(httpContext, oAuthRecordId, error);
            })
            .WithName("SteamLoginFailure");
        
        oAuthGroup.MapGet("/UserData", async (
            HttpContext httpContext,
            [FromServices] ISteamClient steamClient) =>
        {
            if (httpContext.User.Identity?.IsAuthenticated != true)
            {
                return Results.Unauthorized();
            }

            var steamId = httpContext.User.FindFirst("SteamId")?.Value
                ?? throw new OAuthException("SteamId not found in claims!");

            var playerData = await steamClient.GetSteamPlayerSummary(steamId);

            return Results.Ok(playerData);
        })
        .WithName("Userdata")
        .RequireAuthorization();

        oAuthGroup.WithOpenApi();
        return group;
    }
}
