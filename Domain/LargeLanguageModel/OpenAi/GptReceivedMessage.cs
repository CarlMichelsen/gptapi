using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.OpenAi;

public class GptReceivedMessage : GptDelta
{
    [JsonPropertyName("role")]
    public string Role { get; init; } = string.Empty;
}