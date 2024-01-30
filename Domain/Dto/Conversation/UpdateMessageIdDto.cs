namespace Domain.Dto.Conversation;

public record UpdateMessageIdDto(
    string TemporaryUserMessageId,
    Guid ReplacementUserMessageId,
    Guid UpcomingResponseMessageId,
    Guid ConversationId);
