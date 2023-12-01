using Domain.Entity;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Pipeline;
using Interface.Service;

namespace BusinessLogic.Pipeline.SendMessage;

public class CreateOrAppendConversationStage : IPipelineStage<SendMessagePipelineParameters>
{
    private readonly IConversationService conversationService;

    public CreateOrAppendConversationStage(
        IConversationService conversationService)
    {
        this.conversationService = conversationService;
    }

    public async Task<SendMessagePipelineParameters> Process(
        SendMessagePipelineParameters input,
        CancellationToken cancellationToken)
    {
        Domain.Result<Conversation, string> conversationResult;
        if (input.ConversationId is null)
        {
            conversationResult = await this.conversationService
                .StartConversation(input.UserProfileId, input.UserMessage);
        }
        else
        {
            conversationResult = await this.conversationService
                .AppendConversation(input.UserProfileId, input.ConversationId, input.UserMessage);
        }

        var conversation = conversationResult.Match(
            (conversation) => conversation,
            (error) => throw new PipelineException(error));
        
        input.Conversation = conversation;
        return input;
    }
}
