using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.Claude.Stream;

public class ClaudeOutputUsage
{
    [JsonPropertyName("output_tokens")]
    public required int OutputTokens { get; init; }
}