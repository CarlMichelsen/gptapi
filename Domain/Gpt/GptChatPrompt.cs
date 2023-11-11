using System.Text.Json.Serialization;

namespace Domain.Gpt;

public class GptChatPrompt
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("messages")]
    public required List<GptChatMessage> Messages { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the request should be streamed or just awaited.
    /// This value is overwritten by GptChatClient depending on what method of prompting is used.
    /// </summary>
    [JsonPropertyName("stream")]
    public bool Stream { get; set; }
}