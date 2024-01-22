using Domain.Exception;
using Interface.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic.Pipeline;

public abstract class PipelineSingleton<T> : IPipeline<T>
{
    private readonly List<Type> stageTypes = new();
    private readonly IServiceProvider serviceProvider;

    protected PipelineSingleton(IServiceProvider serviceProvider) =>
        this.serviceProvider = serviceProvider;

    public async Task<T> Execute(
        T input,
        CancellationToken cancellationToken)
    {
        using var scope = this.serviceProvider.CreateScope();
        var current = input;
        
        foreach (var stageType in this.stageTypes)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var obj = scope.ServiceProvider.GetRequiredService(stageType);
                if (obj is null)
                {
                    throw new PipelineException(
                        $"Failed to instantiate PipelineStage of type {stageType.Name}");
                }

                current = await (obj as IPipelineStep<T>)!.Process(current, cancellationToken);
            }
            catch (PipelineException e)
            {
                var pipelineName = this.GetType().Name;
                var stageName = stageType.GetType().Name;
                throw new PipelineException(
                    $"an exception was trown in pipeline <{pipelineName}> by {stageName}", e);
            }
        }

        return current;
    }

    public PipelineSingleton<T> AddStageType(Type pipelineStage)
    {
        if (pipelineStage.IsAssignableFrom(typeof(IPipelineStep<T>)))
        {
            throw new PipelineException(
                "Attempted to register a pipelineStage that is does not implement IPipelineStage");
        }

        this.stageTypes.Add(pipelineStage);
        
        return this;
    }
}
