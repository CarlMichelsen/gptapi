using System.Text.Json.Serialization;
using Domain.Converter;
using Domain.Exception;
using Domain.LargeLanguageModel.OpenAi.Map;
using Domain.LargeLanguageModel.Shared.Interface;
using Domain.LargeLanguageModel.Shared.Stream;

namespace Domain.LargeLanguageModel.OpenAi;

public class GptChatStreamChunk : ILlmChunkConvertible
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

    public LlmChunk Convert()
    {
        try
        {
            return GptMapper.Map(this);
        }
        catch (System.Exception e)
        {
            throw new LargeLanguageModelException("Failed converting GptChatStreamChunk to LargeLanguageModelChunk", e);
        }
    }
}