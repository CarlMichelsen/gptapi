using Domain;
using Interface.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class SessionEndpoints
{
    public static RouteGroupBuilder MapSessionEndpoints(this RouteGroupBuilder group)
    {
        var sessionGroup = group.MapGroup("/session");

        sessionGroup.MapPost(
            "/UserData",
            [Authorize(Policy = GptApiConstants.RequireSessionAuthorize)]
            async ([FromServices] ISessionHandler sessionHandler) =>
        {
            var userDataResult = await sessionHandler.GetUserData();
            return userDataResult.Match(
                (playerData) => Results.Ok(playerData),
                (error) => Results.StatusCode((int)error));
        })
        .WithName("Userdata")
        .RequireAuthorization();

        return group;
    }
}