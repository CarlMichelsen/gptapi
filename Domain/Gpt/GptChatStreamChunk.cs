using System.Text.Json.Serialization;
using Domain.Converter;

namespace Domain.Gpt;

public class GptChatStreamChunk
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("object")]
    public required string Object { get; init; }

    [JsonPropertyName("created")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public required DateTime Created { get; init; }

    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("system_fingerprint")]
    public string? SystemFingerprint { get; init; }

    [JsonPropertyName("choices")]
    public required List<GptStreamChoice> Choices { get; init; }
}