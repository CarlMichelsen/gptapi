using Interface.Handler;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class SessionEndpoints
{
    public static RouteGroupBuilder MapSessionEndpoints(this RouteGroupBuilder group)
    {
        var sessionGroup = group.MapGroup("/session");

        sessionGroup.MapPost("/UserData", async (
            [FromServices] ISessionHandler sessionHandler) =>
        {
            var userDataResult = await sessionHandler.GetUserData();

            return userDataResult.Match(
                (playerData) => Results.Ok(playerData),
                (error) => Results.StatusCode((int)error));
        })
        .WithName("Userdata")
        .RequireAuthorization();

        sessionGroup.MapDelete("/Logout", async (
            [FromServices] ISessionHandler sessionHandler) =>
        {
            var result = await sessionHandler.Logout();

            return result.Match(
                (_) => Results.Ok(),
                (error) => Results.StatusCode((int)error));
        })
        .WithName("Logout")
        .RequireAuthorization();

        return group;
    }
}
