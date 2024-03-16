using Domain;
using Interface.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class AvailableModelEndpoint
{
    public static RouteGroupBuilder MapAvailableModelEndpoints(this RouteGroupBuilder group)
    {
        var avaliableModelGroup = group.MapGroup("/availablemodel");

        avaliableModelGroup.MapGet(
            "/",
            [Authorize(Policy = GptApiConstants.SessionAuthenticationScheme)]
            async ([FromServices] IAvailableModelHandler availableModelHandler) =>
            await availableModelHandler.GetAvailableModels())
            .WithName("AvailableModel")
            .RequireAuthorization();

        return group;
    }
}