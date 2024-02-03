using Domain.Entity;
using Domain.Entity.Id;

namespace Interface.Factory;

public interface IConversationTemplateFactory
{
    Task<Conversation> CreateConversation(
        Guid userProfileId,
        ConversationId conversationId,
        Message message);
    
    Conversation CreateConversationForSummaryPrompt(
        Conversation exsistingConv);
}
