namespace Domain.Dto.Conversation;

public class FirstMessageDto
{
    public required Guid ConversationId { get; init; }
    
    public required MessageDto Message { get; init; }
}
