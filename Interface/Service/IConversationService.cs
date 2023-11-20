using Domain;
using Domain.Dto.Conversation;
using Domain.Entity;

namespace Interface.Service;

public interface IConversationService
{
    Task<Result<List<ConversationMetaDataDto>, string>> GetConversations(string userId);
    
    Task<Result<Conversation, string>> GetConversation(string userId, Guid conversationId);

    Task<Result<Conversation, string>> AppendConversation(string userId, Guid conversationId, Message message);
    
    Task<Result<Conversation, string>> StartConversation(string userId, Message message);

    Task<bool> SetConversationSummary(string userId, Guid conversationId, string summary);
}
