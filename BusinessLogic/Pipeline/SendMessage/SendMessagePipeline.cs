using BusinessLogic.Pipeline.SendMessage.Message;
using Domain.Abstractions;
using Domain.Pipeline.SendMessage;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Pipeline.SendMessage;

public sealed class SendMessagePipeline : Pipeline<SendMessagePipelineContext>
{
    private readonly ILogger<SendMessagePipeline> logger;
    private DateTime? dateTime = default;

    public SendMessagePipeline(
        ILogger<SendMessagePipeline> logger,
        IdentifyConversationStep identifyConversationStep,
        InitiateNewMessageStep initiateNewMessage,
        StreamGptResponseStep streamGptResponseStep,
        CompleteGptResponseMessageStep completeGptResponseMessageStep)
        : base(
            identifyConversationStep,
            initiateNewMessage,
            streamGptResponseStep,
            completeGptResponseMessageStep)
    {
        this.logger = logger;
    }

    protected override void PrePipelineExecution(
        SendMessagePipelineContext context,
        CancellationToken cancellationToken)
    {
        this.dateTime = DateTime.UtcNow;
        this.logger.LogInformation("Starting <SendMessagePipeline> execution for message");
    }

    protected override void PostPipelineExecution(
        Result<SendMessagePipelineContext> result,
        CancellationToken cancellationToken)
    {
        if (result.IsError)
        {
            return;
        }
        
        var timespan = DateTime.UtcNow - this.dateTime;

        this.logger.LogInformation(
            "Completed <SendMessagePipeline> execution for message\nExecution time: {ms} ms",
            timespan!.Value.Milliseconds);
    }
}