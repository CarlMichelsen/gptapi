using Domain.Exception;
using Interface.Factory;
using Interface.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic.Factory;

public class PipelineStageFactory : IPipelineStageFactory
{
    private readonly IServiceProvider serviceProvider;

    public PipelineStageFactory(
        IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public T Create<T>()
        where T : IPipelineStage, new()
    {
        var type = typeof(T);
        var pipelineStage = this.serviceProvider.GetRequiredService(type);
        if (pipelineStage is null)
        {
            throw new FactoryException($"Failed to create IPipelineStage of type {type.Name}");
        }

        return (T)pipelineStage;
    }
}
