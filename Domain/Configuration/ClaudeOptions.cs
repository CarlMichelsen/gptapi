namespace Domain.Configuration;

/// <summary>
/// ApplicationOptions, not configured from configuration section.
/// </summary>
public class ClaudeOptions
{
    public const string SectionName = "Claude";

    /// <summary>
    /// Gets or sets list of Claude api keys.
    /// </summary>
    public required List<string> ApiKeys { get; set; }
}
