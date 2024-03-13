using System.Text.Json.Serialization;
using Domain.LargeLanguageModel.Claude.Map;
using Domain.LargeLanguageModel.Shared;
using Domain.LargeLanguageModel.Shared.Interface;

namespace Domain.LargeLanguageModel.Claude;

public class ClaudeResponse : ILargeLanguageModelResponseConvertible
{
    [JsonPropertyName("content")]
    public required List<ClaudeResponseContent> Content { get; init; }

    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("stop_reason")]
    public required string? StopReason { get; init; }

    [JsonPropertyName("stop_sequence")]
    public required string? StopSequence { get; init; }

    [JsonPropertyName("usage")]
    public required ClaudeUsage Usage { get; init; }

    public LargeLanguageModelResponse Convert()
    {
        return ClaudeResponseMapper.Map(this);
    }
}
