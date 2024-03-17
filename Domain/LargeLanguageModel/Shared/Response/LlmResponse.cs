using Domain.LargeLanguageModel.Shared.Request;

namespace Domain.LargeLanguageModel.Shared.Response;

public class LlmResponse
{
    public required string Id { get; init; }

    public required LlmModelVersion Model { get; init; }

    public required List<LlmContent> Choices { get; init; }

    public required LlmUsage Usage { get; init; }
}