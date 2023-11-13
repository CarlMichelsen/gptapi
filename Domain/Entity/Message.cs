namespace Domain.Entity;

public class Message
{
    public required Guid Id { get; init; }
    
    public required string? ResponseId { get; set; }

    public required Role Role { get; init; }

    /// <summary>
    /// Gets or sets content of the message.
    /// It is useful to be able to set content continously as the response is streaming in.
    /// </summary>
    /// <value>String message content.</value>
    public required string Content { get; set; }
}
