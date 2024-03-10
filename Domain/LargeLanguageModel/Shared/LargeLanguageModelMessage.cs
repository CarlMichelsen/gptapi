namespace Domain.LargeLanguageModel.Shared;

public class LargeLanguageModelMessage
{
    public required LargeLanguageModelMessageRole Role { get; init; }

    public required string Content { get; init; }
}
