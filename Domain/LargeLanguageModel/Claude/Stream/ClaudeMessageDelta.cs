using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.Claude.Stream;

public class ClaudeMessageDelta
{
    [JsonPropertyName("stop_reason")]
    public required string StopReason { get; init; }

    [JsonPropertyName("stop_sequence")]
    public required string? StopSequence { get; init; }
}