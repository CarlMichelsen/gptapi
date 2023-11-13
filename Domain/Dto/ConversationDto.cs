namespace Domain.Dto;

public class ConversationDto
{
    public required Guid Id { get; init; }

    public required List<MessageDto> Messages { get; init; }
}
