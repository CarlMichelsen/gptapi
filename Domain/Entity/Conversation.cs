using Domain.Entity.Id;

namespace Domain.Entity;

public class Conversation
{
    public required ConversationId Id { get; init; }

    public required Guid UserProfileId { get; init; }

    public string? Summary { get; set; }

    public required List<Message> Messages { get; init; }

    public required DateTime CreatedUtc { get; init; }

    public DateTime LastAppendedUtc { get; set; } = DateTime.UtcNow;

    public bool UserArchived { get; set; } = false;
}
