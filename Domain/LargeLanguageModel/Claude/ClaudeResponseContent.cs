using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.Claude;

public class ClaudeResponseContent
{
    [JsonPropertyName("text")]
    public required string Text { get; init; }

    [JsonPropertyName("type")]
    public required string Type { get; init; }
}
