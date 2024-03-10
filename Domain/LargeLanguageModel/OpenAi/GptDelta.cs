using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.OpenAi;

public class GptDelta
{
    [JsonPropertyName("content")]
    public string Content { get; init; } = string.Empty;
}
