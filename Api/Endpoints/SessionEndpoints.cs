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
            [Authorize(Policy = GptApiConstants.SessionAuthenticationScheme)]
            async ([FromServices] ISessionHandler sessionHandler) => await sessionHandler.GetUserData())
        .WithName("Userdata")
        .AllowAnonymous();

        return group;
    }
}