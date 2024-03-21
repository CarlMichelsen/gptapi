using System.Text.Json.Serialization;
using Domain.Exception;
using Domain.LargeLanguageModel.Claude.Map;
using Domain.LargeLanguageModel.Shared.Interface;
using Domain.LargeLanguageModel.Shared.Stream;

namespace Domain.LargeLanguageModel.Claude.Stream.Event;

public class EventContentBlockDelta : ClaudeUnknownEventBase, IClaudeEvent, ILlmChunkConvertible
{
    public ClaudeStreamEventType Type => ClaudeStreamEventType.ContentBlockDelta;

    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required ClaudeContentBlock Delta { get; init; }

    public LlmChunk Convert(Guid streamIdentifier)
    {
        try
        {
            return ClaudeResponseMapper.Map(this, streamIdentifier);
        }
        catch (System.Exception e)
        {
            throw new LargeLanguageModelException("Failed converting EventContentBlockDelta to LlmChunk", e);
        }
    }
}