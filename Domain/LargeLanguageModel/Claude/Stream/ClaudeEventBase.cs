using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.Claude.Stream;

public abstract class ClaudeEventBase
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }
}
