namespace Domain.Dto.Conversation;

public class SendMessageRequest
{
    public required Guid? ConversationId { get; init; }

    public required string MessageContent { get; init; }

    public required Guid? PreviousMessageId { get; init; }
}
