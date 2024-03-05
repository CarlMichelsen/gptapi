using Domain.Abstractions;
using Domain.Dto.Conversation;
using Domain.Entity;
using Domain.Entity.Id;

namespace Interface.Service;

public interface IConversationService
{
    Task<Result<List<ConversationDateChunkDto>>> GetConversationList(
        Guid userProfileId);
    
    Task<Result<Conversation>> GetConversation(
        Guid userProfileId,
        ConversationId conversationId);

    Task<Result<bool>> DeleteConversation(
        Guid userProfileId,
        ConversationId conversationId);

    Task<Result<bool>> SetConversationSummary(
        Guid userProfileId,
        ConversationId conversationId,
        string summary);
}
