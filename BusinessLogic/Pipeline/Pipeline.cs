using Domain.Exception;
using Interface.Pipeline;

namespace BusinessLogic.Pipeline;

public abstract class Pipeline<T>
{
    private readonly List<IPipelineStage<T>> stages = new();

    public virtual async Task<T> Execute(T input, CancellationToken cancellationToken)
    {
        var current = input;
        foreach (var stage in this.stages)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                current = await stage.Process(current, cancellationToken);
            }
            catch (PipelineException e)
            {
                var pipelineName = this.GetType().Name;
                var stageName = stage?.GetType().Name;
                throw new PipelineException(
                    $"Stage <{stageName}> thew an exception in pipeline <{pipelineName}>", e);
            }
        }

        return current;
    }

    protected Pipeline<T> AddStage(IPipelineStage<T> stage)
    {
        this.stages.Add(stage);
        return this;
    }

    protected void ClearStages()
    {
        this.stages.Clear();
    }
}
