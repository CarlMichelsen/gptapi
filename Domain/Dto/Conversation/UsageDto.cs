namespace Domain.Dto.Conversation;

public class UsageDto
{
    public required string Provider { get; init; }

    public required string Model { get; init; }

    /// <summary>
    /// Gets the completion token amount for this message.
    /// </summary>
    public required int Tokens { get; init; }
}
