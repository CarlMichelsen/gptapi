namespace Domain.Entity;

public sealed class Usage
{
    public int Id { get; set; }

    public required LlmProvider Provider { get; init; }

    public required string Model { get; init; }

    /// <summary>
    /// Gets the completion token amount for this message.
    /// </summary>
    public required int Tokens { get; init; }
}
