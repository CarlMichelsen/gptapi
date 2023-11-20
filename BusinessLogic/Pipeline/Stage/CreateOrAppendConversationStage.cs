using Domain.Entity;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Pipeline;
using Interface.Service;

namespace BusinessLogic.Pipeline.Stage;

public class CreateOrAppendConversationStage : IPipelineStage<SendMessagePipelineParameter>
{
    private readonly IConversationService conversationService;

    public CreateOrAppendConversationStage(
        IConversationService conversationService)
    {
        this.conversationService = conversationService;
    }

    public async Task<SendMessagePipelineParameter> Process(
        SendMessagePipelineParameter input,
        CancellationToken cancellationToken)
    {
        Domain.Result<Conversation, string> conversationResult;
        if (input.ConversationId is null || input.ConversationId == Guid.Empty)
        {
            conversationResult = await this.conversationService
                .StartConversation(input.UserId, input.UserMessage);
        }
        else
        {
            if (input.UserMessage.Id != Guid.Empty)
            {
                throw new PipelineException(
                    $"UserMessage should not have an Id here under any circumstances but here it is {input.UserMessage.Id}");
            }
            
            var notNullConversationId = Guid.Parse(input.ConversationId!.ToString()!);
            conversationResult = await this.conversationService
                .AppendConversation(input.UserId, notNullConversationId, input.UserMessage);
        }

        var conversation = conversationResult.Match(
            (conversation) => conversation,
            (error) => throw new PipelineException(error));
        
        input.Conversation = conversation;
        return input;
    }
}
