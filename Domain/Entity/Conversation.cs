namespace Domain.Entity;

public class Conversation
{
    public required Guid Id { get; init; }

    public required List<Message> Messages { get; init; }
}
