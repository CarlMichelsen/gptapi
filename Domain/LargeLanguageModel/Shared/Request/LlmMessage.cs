namespace Domain.LargeLanguageModel.Shared.Request;

public class LlmMessage
{
    public required LlmRole Role { get; init; }

    public required string Content { get; init; }
}
