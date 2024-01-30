using Domain;
using Domain.Dto.Conversation;
using Domain.Entity.Id;
using Interface.Handler;

namespace BusinessLogic.Handler;

public class ConversationHandler : IConversationHandler
{
    public Task<DeprecatedResult<bool, string>> DeleteConversation(ConversationId conversationId)
    {
        throw new NotImplementedException();
    }

    public Task<DeprecatedResult<ConversationDto, string>> GetConversation(ConversationId conversationId)
    {
        throw new NotImplementedException();
    }

    public Task<DeprecatedResult<List<ConversationMetaDataDto>, string>> GetConversations()
    {
        throw new NotImplementedException();
    }
}
