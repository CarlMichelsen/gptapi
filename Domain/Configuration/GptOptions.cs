namespace Domain.Configuration;

public class GptOptions
{
    public const string SectionName = "Gpt";

    public required List<string> ApiKeys { get; init; }
}
