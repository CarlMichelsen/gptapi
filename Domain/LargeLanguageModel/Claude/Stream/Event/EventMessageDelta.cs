using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.Claude.Stream.Event;

public class EventMessageDelta : ClaudeEventBase
{
    [JsonPropertyName("delta")]
    public required ClaudeMessageDelta Delta { get; init; }
}
