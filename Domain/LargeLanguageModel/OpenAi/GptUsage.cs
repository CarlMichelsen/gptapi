using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.OpenAi;

public class GptUsage
{
    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}
