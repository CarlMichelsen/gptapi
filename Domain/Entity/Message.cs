using Domain.Entity.Id;

namespace Domain.Entity;

public class Message
{
    public required MessageId Id { get; set; }

    public required Message? PreviousMessage { get; set; }
    
    public string? ResponseId { get; set; }

    public bool Visible { get; init; } = true;

    public required Role Role { get; init; }

    public bool Complete { get; set; } = false;

    /// <summary>
    /// Gets or sets content of the message.
    /// It is useful to be able to set content continously as the response is streaming in.
    /// </summary>
    /// <value>String message content.</value>
    public required string Content { get; set; }

    public required DateTime Created { get; init; }
}
