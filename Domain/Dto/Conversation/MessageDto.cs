namespace Domain.Dto.Conversation;

public class MessageDto
{
    public required Guid Id { get; init; }

    public required string Role { get; init; }

    public required string Content { get; init; }
}
