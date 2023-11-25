namespace Domain.Dto.Conversation;

public class ConversationDto
{
    public required Guid Id { get; init; }

    public required string? Summary { get; set; }

    public required List<MessageDto> Messages { get; init; }

    public required DateTime LastAppended { get; init; }
}
