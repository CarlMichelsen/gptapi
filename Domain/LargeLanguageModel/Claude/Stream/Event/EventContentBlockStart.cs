using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.Claude.Stream.Event;

public class EventContentBlockStart : ClaudeUnknownEventBase, IClaudeEvent
{
    public ClaudeStreamEventType Type => ClaudeStreamEventType.ContentBlockStart;

    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("content_block")]
    public required ClaudeContentBlock ContentBlock { get; init; }
}