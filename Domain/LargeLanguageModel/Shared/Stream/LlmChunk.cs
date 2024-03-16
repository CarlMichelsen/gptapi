using Domain.LargeLanguageModel.Shared.Response;

namespace Domain.LargeLanguageModel.Shared.Stream;

public class LlmChunk
{
    public required List<LlmContent> Choices { get; init; }
}