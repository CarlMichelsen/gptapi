using System.Text.Json.Serialization;
using Domain.LargeLanguageModel.Claude.Stream;

namespace Domain;

public class ClaudeMessageDelta
{
    [JsonPropertyName("stop_reason")]
    public required string StopReason { get; init; }

    [JsonPropertyName("stop_sequence")]
    public required string? StopSequence { get; init; }

    [JsonPropertyName("usage")]
    public required ClaudeOutputUsage Usage { get; init; }
}