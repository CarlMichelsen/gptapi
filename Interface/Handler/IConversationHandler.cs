using Domain;
using Domain.Dto.Conversation;
using Domain.Entity.Id;

namespace Interface.Handler;

public interface IConversationHandler
{
    Task<Result<List<ConversationMetaDataDto>, string>> GetConversations();

    Task<Result<ConversationDto, string>> GetConversation(ConversationId conversationId);

    Task<Result<bool, string>> DeleteConversation(ConversationId conversationId);
}
