using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.Claude.Stream.Event;

public class EventContentBlockStop : ClaudeUnknownEventBase, IClaudeEvent
{
    public ClaudeStreamEventType Type => ClaudeStreamEventType.ContentBlockStop;

    [JsonPropertyName("index")]
    public required int Index { get; init; }
}
