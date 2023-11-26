using System.Reflection;
using BusinessLogic.Pipeline;
using Interface.Pipeline;

namespace Api.Extensions;

public static class PipelineStageRegistrations
{
    public static IServiceCollection RegisterPipelineStages(
        this IServiceCollection services)
    {
        var assembly = Assembly.GetAssembly(typeof(Pipeline<>))
            ?? throw new Exception("Failed to find assembly for PipelineStage registration");
        
        var implementations = assembly
            .GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract)
            .Where(type => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineStage<>)))
            .ToList();

        foreach (var implementation in implementations)
        {
            services.AddTransient(implementation);
        }

        return services;
    }
}
