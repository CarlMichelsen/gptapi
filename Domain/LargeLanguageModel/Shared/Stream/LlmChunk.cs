using Domain.LargeLanguageModel.Shared.Response;

namespace Domain.LargeLanguageModel.Shared.Stream;

public class LlmChunk
{
    public required Guid StreamIdentifier { get; init; }
    
    public required List<LlmContent> Choices { get; init; }
}