namespace Domain.Dto.Conversation;

public class ReceiveMessageDto
{
    public required Guid ConversationId { get; init; }
    
    public required MessageDto Message { get; init; }
}
