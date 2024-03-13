using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.Claude.Stream.Event;

public class EventContentBlockDelta : ClaudeEventBase
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required ClaudeContentBlock Delta { get; init; }
}