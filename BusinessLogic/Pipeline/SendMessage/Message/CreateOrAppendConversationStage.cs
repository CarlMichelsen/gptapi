using Domain.Entity;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Pipeline;
using Interface.Service;

namespace BusinessLogic.Pipeline.SendMessage.Message;

public class CreateOrAppendConversationStage : IPipelineStep<SendMessagePipelineParameters>
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
            try
            {
                conversationResult = await this.conversationService
                    .AppendConversation(input.UserProfileId, input.ConversationId, input.UserMessage);
            }
            catch (ServiceException e)
            {
                throw new PipelineException("Service threw an exception inside SendMessagePipeline", e);
            }
        }

        var conversation = conversationResult.Match(
            (conversation) => conversation,
            (error) => throw new PipelineException(error));
        
        input.Conversation = conversation;
        return input;
    }
}
