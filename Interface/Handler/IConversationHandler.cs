using Domain;
using Domain.Dto.Conversation;
using Domain.Entity.Id;

namespace Interface.Handler;

public interface IConversationHandler
{
    Task<DeprecatedResult<List<ConversationMetaDataDto>, string>> GetConversations();

    Task<DeprecatedResult<ConversationDto, string>> GetConversation(ConversationId conversationId);

    Task<DeprecatedResult<bool, string>> DeleteConversation(ConversationId conversationId);
}
