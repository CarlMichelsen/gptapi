using Domain;
using Domain.Exception;
using Interface.Pipeline;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler;

public abstract class BasePipelineExecutorHandler
{
    protected async Task<Result<T, string>> ExecutePipeline<T>(
        ILogger logger,
        IPipeline<T> pipeline,
        T parameters,
        string methodName,
        CancellationToken cancellationToken)
    {
        string error;

        try
        {
            return await pipeline.Execute(parameters, cancellationToken);
        }
        catch (OperationCanceledException e)
        {
            error = e.Message;
            logger.LogWarning(
                "{method} method was cancelled {e}",
                methodName,
                e);
        }
        catch (PipelineException e)
        {
            error = e.Message;
            logger.LogError(
                "A PipelineException happened in the {method} method {e}",
                methodName,
                e);
        }
        catch (Exception e)
        {
            error = e.Message;
            logger.LogCritical(
                "A critical exception happened in the {method} method {e}",
                methodName,
                e);
        }

        return error;
    }
}
