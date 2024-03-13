using Domain.Entity.Id;

namespace Domain.Entity;

public sealed class Message
{
    public required MessageId Id { get; set; }
    
    public required Message? PreviousMessage { get; set; }

    public required Role Role { get; init; }

    public required string Content { get; set; }

    public required DateTime CreatedUtc { get; init; }

    public required Usage? Usage { get; set; }

    public bool Visible { get; init; } = true;

    public DateTime? CompletedUtc { get; set; }
}
