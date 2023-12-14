using BusinessLogic.Pipeline.SendMessage.Message;
using Domain.Pipeline;

namespace BusinessLogic.Pipeline.SendMessage;

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
