using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.Claude;

public class ClaudeMessage
{
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("content")]
    public required string Content { get; init; }
}
