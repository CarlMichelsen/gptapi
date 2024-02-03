using Domain.Entity.Id;

namespace Domain.Pipeline.SendMessage;

public record ConversationAppendData(
    MessageId ExsistingMessageId,
    ConversationId ExsistingConversationId);