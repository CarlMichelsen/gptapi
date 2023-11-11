namespace Domain.Configuration;

public class GptOptions
{
    public const string SectionName = "Gpt";
    
    public required string ApiKey { get; init; }
}
