using System.Text.Json.Serialization;

namespace Domain.Gpt;

public class GptReceivedMessage
{
    [JsonPropertyName("role")]
    public string Role { get; init; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; init; } = string.Empty;
}