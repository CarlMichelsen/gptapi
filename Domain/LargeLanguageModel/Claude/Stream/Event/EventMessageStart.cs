using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.Claude.Stream.Event;

public class EventMessageStart : ClaudeUnknownEventBase, IClaudeEvent
{
    public ClaudeStreamEventType Type => ClaudeStreamEventType.MessageStart;
    
    [JsonPropertyName("message")]
    public required ClaudeResponse Message { get; init; }
}