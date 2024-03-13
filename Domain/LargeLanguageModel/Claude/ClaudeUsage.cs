using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.Claude;

public class ClaudeUsage
{
    [JsonPropertyName("input_tokens")]
    public required int InputTokens { get; init; }

    [JsonPropertyName("output_tokens")]
    public required int OutputTokens { get; init; }
}
