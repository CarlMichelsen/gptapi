using System.Text.Json.Serialization;
using Domain.LargeLanguageModel.Shared.Interface;

namespace Domain.LargeLanguageModel.Claude;

public class ClaudePrompt : ILargeLanguageModelRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("max_tokens")]
    public required int MaxTokens { get; init; }

    [JsonPropertyName("messages")]
    public required List<ClaudeMessage> Messages { get; init; }

    [JsonPropertyName("stream")]
    public bool Stream { get; set; }
}