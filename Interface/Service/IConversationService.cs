using Domain;
using Domain.Dto.Conversation;
using Domain.Entity;
using Domain.Entity.Id;

namespace Interface.Service;

public interface IConversationService
{
    Task<Result<List<ConversationMetaDataDto>, string>> GetConversations(
        UserProfileId userProfileId);
    
    Task<Result<Conversation, string>> GetConversation(
        UserProfileId userProfileId,
        ConversationId conversationId);

    Task<Result<bool, string>> DeleteConversation(
        UserProfileId userProfileId,
        ConversationId conversationId);

    Task<Result<Conversation, string>> AppendConversation(
        UserProfileId userProfileId,
        ConversationId conversationId,
        Message message);
    
    Task<Result<Conversation, string>> StartConversation(
        UserProfileId userProfileId,
        Message message);

    Task<bool> SetConversationSummary(
        UserProfileId userProfileId,
        ConversationId conversationId,
        string summary);
}
