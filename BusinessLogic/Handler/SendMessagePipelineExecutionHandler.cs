using BusinessLogic.Pipeline.SendMessage;
using Domain.Abstractions;
using Domain.Pipeline;
using Domain.Pipeline.SendMessage;
using Interface.Factory;
using Interface.Handler;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler;

public class SendMessagePipelineExecutionHandler : ISendMessagePipelineExecutionHandler
{
    public static Dictionary<Guid, PipelineTracker> PipelineTrackerDictionary = new();
    private readonly ILogger<SendMessagePipelineExecutionHandler> logger;
    private readonly IScopedServiceFactory scopedServiceFactory;

    public SendMessagePipelineExecutionHandler(
        ILogger<SendMessagePipelineExecutionHandler> logger,
        IScopedServiceFactory scopedServiceFactory)
    {
        this.logger = logger;
        this.scopedServiceFactory = scopedServiceFactory;
    }

    public Guid SendMessage(SendMessagePipelineContext context, Action<Error> errorHandler)
    {
        var source = new CancellationTokenSource();
        var pipelineTracker = new PipelineTracker
        {
            PipelineIdentifier = context.PipelineIdentifier,
            CancellationTokenSource = source,
            PipelineTask = this.RunSendMessagePipeline(context, errorHandler, source.Token),
        };

        PipelineTrackerDictionary.Add(pipelineTracker.PipelineIdentifier, pipelineTracker);

        return pipelineTracker.PipelineIdentifier;
    }

    public void CancelMessage(Guid pipelineIdentifier)
    {
        if (PipelineTrackerDictionary.TryGetValue(pipelineIdentifier, out var pipelineTracker))
        {
            pipelineTracker.CancellationTokenSource.Cancel();
            PipelineTrackerDictionary.Remove(pipelineIdentifier);
        }
    }

    private async Task<Result<SendMessagePipelineContext>> RunSendMessagePipeline(
        SendMessagePipelineContext initialContext,
        Action<Error> errorHandler,
        CancellationToken cancellationToken)
    {
        var sendMessagePipelineResult = this.scopedServiceFactory
            .CreateScopedService<SendMessagePipeline>();
        if (sendMessagePipelineResult.IsError)
        {
            this.logger.LogError("Unable to create SendMessagePipeline");
        }

        var sendMessagePipeline = sendMessagePipelineResult.Unwrap();
        var result = await sendMessagePipeline.Execute(initialContext, cancellationToken);

        if (result.IsError)
        {
            errorHandler(result.Error!);
        }

        return result;
    }
}
