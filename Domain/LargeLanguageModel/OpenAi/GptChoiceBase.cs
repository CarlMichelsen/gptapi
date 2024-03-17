using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.OpenAi;

public abstract class GptChoiceBase
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("finish_reason")]
    public required string? FinishReason { get; init; }
}