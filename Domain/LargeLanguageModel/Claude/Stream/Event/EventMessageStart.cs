using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.Claude.Stream.Event;

public class EventMessageStart : ClaudeEventBase
{
    [JsonPropertyName("message")]
    public required ClaudeResponse Message { get; init; }
}