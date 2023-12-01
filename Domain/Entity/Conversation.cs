using Domain.Entity.Id;

namespace Domain.Entity;

public class Conversation
{
    public required ConversationId Id { get; init; }

    public required UserProfile UserProfile { get; init; }

    public required string? Summary { get; set; }

    public required List<Message> Messages { get; init; }

    public required DateTime Created { get; init; }

    public DateTime LastAppended { get; set; } = DateTime.UtcNow;

    public bool UserDeleted { get; set; } = false;
}
