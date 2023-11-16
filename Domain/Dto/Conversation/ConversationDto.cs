namespace Domain.Dto.Conversation;

public class ConversationDto
{
    public required Guid Id { get; init; }

    public required List<MessageDto> Messages { get; init; }
}
