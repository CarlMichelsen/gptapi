using Domain;
using Domain.Dto.Conversation;
using Domain.Entity;
using Domain.Entity.Id;

namespace Interface.Service;

public interface IConversationService
{
    Task<DeprecatedResult<List<ConversationMetaDataDto>, string>> GetConversations(
        UserProfileId userProfileId);
    
    Task<DeprecatedResult<Conversation, string>> GetConversation(
        UserProfileId userProfileId,
        ConversationId conversationId);

    Task<DeprecatedResult<bool, string>> DeleteConversation(
        UserProfileId userProfileId,
        ConversationId conversationId);

    Task<DeprecatedResult<Conversation, string>> AppendConversation(
        UserProfileId userProfileId,
        ConversationId conversationId,
        Message message);
    
    Task<DeprecatedResult<Conversation, string>> StartConversation(
        UserProfileId userProfileId,
        Message message);

    Task<bool> SetConversationSummary(
        UserProfileId userProfileId,
        ConversationId conversationId,
        string summary);
}
