using Domain.Dto.Conversation;
using Domain.Entity;
using Interface.Pipeline;

namespace BusinessLogic.Pipeline.Stage;

public class CreateOrAppendConversationStage : IPipelineStage<SendMessageRequest, Conversation>
{
    public Task<Conversation> Process(SendMessageRequest input)
    {
        throw new NotImplementedException();
    }
}
