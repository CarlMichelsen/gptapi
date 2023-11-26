using Domain.Exception;
using Interface.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic.Pipeline;

public abstract class PipelineSingleton<T> : Pipeline<T>
{
    private readonly List<Type> stageTypes = new();
    private readonly IServiceProvider serviceProvider;

    protected PipelineSingleton(
        IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public override async Task<T> Execute(T input, CancellationToken cancellationToken)
    {
        using (var scope = this.serviceProvider.CreateScope())
        {
            try
            {
                this.ClearStages();
                var scopedServiceProvider = scope.ServiceProvider;

                foreach (var stageType in this.stageTypes)
                {
                    var obj = scopedServiceProvider.GetRequiredService(stageType);
                    if (obj is null)
                    {
                        throw new PipelineException(
                            $"Failed to instantiate PipelineStage of type {stageType.Name}");
                    }

                    var stage = (IPipelineStage<T>)obj;
                    this.AddStage(stage);
                }

                var pipelineExecution = await base.Execute(input, cancellationToken);

                return pipelineExecution;
            }
            finally
            {
                this.ClearStages();
            }
        }
    }

    public PipelineSingleton<T> AddTypedStage(Type pipelineStage)
    {
        if (pipelineStage.IsAssignableFrom(typeof(IPipelineStage<T>)))
        {
            throw new PipelineException(
                "Attempted to register a pipelineStage that is does not implement IPipelineStage");
        }

        this.stageTypes.Add(pipelineStage);
        
        return this;
    }
}
