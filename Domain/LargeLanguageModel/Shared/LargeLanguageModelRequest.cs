using Domain.LargeLanguageModel.Shared.Interface;

namespace Domain.LargeLanguageModel.Shared;

public class LargeLanguageModelRequest : ILargeLanguageModelRequest
{
    public required LargeLanguageModelVersion ModelVersion { get; init; }

    public required List<LargeLanguageModelMessage> Messages { get; init; }

    public bool Stream { get; set; }

    public LargeLanguageModelRequest Convert()
    {
        return this;
    }
}
