namespace Domain.Entity;

public class Conversation
{
    public required Guid Id { get; init; }

    public required string UserId { get; init; }

    public required string? Summary { get; set; }

    public required List<Message> Messages { get; init; }

    public required DateTime Created { get; init; }
}
