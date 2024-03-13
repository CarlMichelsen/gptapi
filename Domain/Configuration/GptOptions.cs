namespace Domain.Configuration;

public class GptOptions
{
    public const string SectionName = "Gpt";

    /// <summary>
    /// Gets or sets list of GPT api keys.
    /// </summary>
    public required List<string> ApiKeys { get; set; }
}
