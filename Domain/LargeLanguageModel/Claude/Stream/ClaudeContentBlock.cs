using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.Claude.Stream;

public class ClaudeContentBlock
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("text")]
    public required string Text { get; init; }
}