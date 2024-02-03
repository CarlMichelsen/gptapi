using Domain.Abstractions;
using Interface.Pipeline;

namespace BusinessLogic.Pipeline;

public abstract class Pipeline<T> : IPipeline<T>
{
    private readonly List<IPipelineStep<T>> steps;

    protected Pipeline(params IPipelineStep<T>[] pipelineSteps)
    {
        this.steps = new List<IPipelineStep<T>>(pipelineSteps);
    }

    public async Task<Result<T>> Execute(
        T input,
        CancellationToken cancellationToken)
    {
        this.PrePipelineExecution(input, cancellationToken);

        var firstStep = this.steps.FirstOrDefault();
        if (firstStep is null)
        {
            return new Error("Pipeline.Empty", "No steps present in the pipeline");
        }

        var result = await firstStep.Execute(input, cancellationToken);

        foreach (var step in this.steps.Skip(1))
        {
            if (result.IsError)
            {
                break;
            }
            
            result = await step.Execute(result.Unwrap(), cancellationToken);
        }

        this.PostPipelineExecution(result, cancellationToken);
        return result;
    }

    protected abstract void PrePipelineExecution(
        T context,
        CancellationToken cancellationToken);

    protected abstract void PostPipelineExecution(
        Result<T> result,
        CancellationToken cancellationToken);
}
