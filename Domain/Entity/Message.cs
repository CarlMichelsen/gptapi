using Domain.Entity.Id;

namespace Domain.Entity;

public class Message
{
    public required MessageId Id { get; set; }
    
    public required Message? PreviousMessage { get; set; }
    
    public string? ResponseId { get; set; }

    public bool Visible { get; init; } = true;

    public required Role Role { get; init; }

    public required string Content { get; set; }

    public required DateTime CreatedUtc { get; init; }

    public DateTime? CompletedUtc { get; set; }
}
