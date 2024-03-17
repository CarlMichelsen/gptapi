using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.Claude.Stream;

public abstract class ClaudeUnknownEventBase
{
    [JsonPropertyName("type")]
    public required string TypeString { get; init; }
}
