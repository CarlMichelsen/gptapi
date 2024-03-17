using Domain.LargeLanguageModel.Shared.Interface;

namespace Domain.LargeLanguageModel.Shared.Request;

public class LlmRequest : ILlmRequest
{
    public bool Stream { get; set; }

    public required string? SystemMessage { get; init; }

    public required LlmModelVersion ModelVersion { get; init; }

    public required int MaxTokens { get; init; }

    public required List<LlmMessage> Messages { get; init; }
}
