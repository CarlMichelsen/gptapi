using Domain;
using Domain.Dto.Conversation;

namespace Interface.Handler;

public interface IConversationHandler
{
    Task<Result<List<ConversationMetaDataDto>, string>> GetConversations();

    Task<Result<ConversationDto, string>> GetConversation(Guid conversationId);

    Task<Result<bool, string>> DeleteConversation(Guid conversationId);
}
