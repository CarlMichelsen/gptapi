using System.Reflection;
using BusinessLogic.Pipeline;
using Domain.Exception;
using Interface.Pipeline;

namespace Api.Extensions;

public static class PipelineStageRegistrations
{
    public static IServiceCollection RegisterPipelineStages(
        this IServiceCollection services)
    {
        var assembly = Assembly.GetAssembly(typeof(Pipeline<>))
            ?? throw new ApplicationStartupException("Failed to find assembly for PipelineStage registration");
        
        var implementations = assembly
            .GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract)
            .Where(type => Array.Exists(type.GetInterfaces(), i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineStage<>)))
            .ToList();

        foreach (var implementation in implementations)
        {
            services.AddTransient(implementation);
        }

        return services;
    }
}
