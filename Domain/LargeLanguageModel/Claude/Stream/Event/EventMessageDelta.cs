using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.Claude.Stream.Event;

public class EventMessageDelta : ClaudeUnknownEventBase, IClaudeEvent
{
    public ClaudeStreamEventType Type => ClaudeStreamEventType.MessageDelta;
    
    [JsonPropertyName("delta")]
    public required ClaudeMessageDelta Delta { get; init; }
}
