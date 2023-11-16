namespace Domain.Dto.Conversation;

public class MessageChunkDto
{
    public required Guid Id { get; init; }

    public required int Index { get; init; }

    public required string Role { get; init; }

    public required string Content { get; init; }

    public required DateTime Created { get; init; }
}
