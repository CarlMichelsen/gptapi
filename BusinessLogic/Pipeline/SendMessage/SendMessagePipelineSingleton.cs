using Domain.Pipeline;

namespace BusinessLogic.Pipeline.SendMessage;

public sealed class SendMessagePipelineSingleton : PipelineSingleton<SendMessagePipelineParameters>
{
    public SendMessagePipelineSingleton(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        this.AddTypedStage(typeof(CreateOrAppendConversationStage))
            .AddTypedStage(typeof(NotifyUserOfCreatedMessageStage))
            .AddTypedStage(typeof(StreamChatResponseStage))
            .AddTypedStage(typeof(RegisterMessageResponseStage))
            .AddTypedStage(typeof(EnsureConversationSummaryStage));
    }
}
