using Domain;
using Domain.Abstractions;
using Domain.Dto.Conversation;
using Domain.Entity;
using Domain.Entity.Id;

namespace Interface.Service;

public interface IConversationService
{
    Task<Result<List<ConversationMetaDataDto>>> GetConversationList(
        Guid userProfileId);
    
    Task<Result<Conversation>> GetConversation(
        Guid userProfileId,
        ConversationId conversationId);

    Task<Result<bool>> DeleteConversation(
        Guid userProfileId,
        ConversationId conversationId);

    Task<Result<Conversation>> AppendConversation(
        Guid userProfileId,
        ConversationId conversationId,
        Message message);
    
    Task<Result<Conversation>> StartConversation(
        Guid userProfileId,
        Message message);

    Task<Result<bool>> SetConversationSummary(
        Guid userProfileId,
        ConversationId conversationId,
        string summary);
}
