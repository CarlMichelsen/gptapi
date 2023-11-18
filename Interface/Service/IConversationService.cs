using Database;
using Domain;
using Domain.Dto.Conversation;
using Domain.Entity;

namespace Interface.Service;

public interface IConversationService
{
    Task<Result<List<ConversationMetaDataDto>, string>> GetConversations(ApplicationContext context, string userId);
    
    Task<Result<Conversation, string>> GetConversation(ApplicationContext context, string userId, Guid conversationId);

    Task<Result<Conversation, string>> AppendConversation(ApplicationContext context, string userId, Guid conversationId, Message message);
    
    Task<Result<Conversation, string>> StartConversation(ApplicationContext context, string userId, Message message);
}
