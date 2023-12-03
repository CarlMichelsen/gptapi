namespace Api.Endpoints;

public static class HealthCheckEndpoints
{
    public static RouteGroupBuilder MapHealthCheckEndpoints(this RouteGroupBuilder group)
    {
        group
            .MapGroup("/healthcheck")
            .MapGet("/", () => Results.Ok())
            .WithName("HealthCheck");
        
        return group;
    }
}
