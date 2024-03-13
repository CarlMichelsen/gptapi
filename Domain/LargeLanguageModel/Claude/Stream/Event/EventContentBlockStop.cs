using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.Claude.Stream.Event;

public class EventContentBlockStop : ClaudeEventBase
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }
}
