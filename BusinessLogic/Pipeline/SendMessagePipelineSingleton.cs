using BusinessLogic.Pipeline.SendMessage;
using Domain.Pipeline;

namespace BusinessLogic.Pipeline;

public sealed class SendMessagePipelineSingleton : PipelineSingleton<SendMessagePipelineParameters>
{
    public SendMessagePipelineSingleton(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        this.AddStageType(typeof(CreateOrAppendConversationStage))
            .AddStageType(typeof(NotifyUserOfCreatedMessageStage))
            .AddStageType(typeof(StreamChatResponseStage))
            .AddStageType(typeof(RegisterMessageResponseStage))
            .AddStageType(typeof(EnsureConversationSummaryStage));
    }
}
