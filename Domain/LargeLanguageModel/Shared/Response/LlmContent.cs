using Domain.LargeLanguageModel.Shared.Request;

namespace Domain.LargeLanguageModel.Shared.Response;

public class LlmContent
{
    public required LlmRole Role { get; init; }

    public required string Content { get; init; }

    public required string? StopReason { get; init; }
}
