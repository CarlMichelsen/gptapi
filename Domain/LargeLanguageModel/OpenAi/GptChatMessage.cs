using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.OpenAi;

public class GptChatMessage
{
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("content")]
    public required string Content { get; init; }
}
