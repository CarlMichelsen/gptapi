using Domain;
using Interface.Handler;
using Microsoft.AspNetCore.Mvc;

namespace Api;

public static class DevelopmentIdpEndpoints
{
    public static WebApplication MapDevelopmentIdpEndpoints(this WebApplication app)
    {
        app.MapGet($"/{GptApiConstants.DeveloperIdpName}", async context =>
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync("Idp/index.html");
        })
        .WithName(GptApiConstants.DeveloperIdpName);

        app.MapGet("/DevelopmentUsers", async ([FromServices] IDevelopmentIdpHandler developmentIdpHandler) =>
            await developmentIdpHandler.GetDevelopmentUsers());

        return app;
    }
}
