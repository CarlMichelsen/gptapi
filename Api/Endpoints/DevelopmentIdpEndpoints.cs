using Domain;
using Interface.Provider;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class DevelopmentIdpEndpoints
{
    public static RouteGroupBuilder MapDevelopmentIdpEndpoints(this WebApplication app)
    {
        var devGroup = app.MapGroup("/development");
        
        devGroup.MapGet($"/{GptApiConstants.DeveloperIdpName}", async context =>
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync("Idp/index.html");
        })
        .WithName(GptApiConstants.DeveloperIdpName);

        devGroup.MapGet("/DevelopmentUsers", async (
            HttpContext context,
            [FromServices] IDevelopmentIdentityProvider developmentIdpHandler) =>
        {
            var developmentUsers = await developmentIdpHandler.GetDevelopmentUsers();
            return Results.Ok(developmentUsers);
        });

        return devGroup;
    }
}
