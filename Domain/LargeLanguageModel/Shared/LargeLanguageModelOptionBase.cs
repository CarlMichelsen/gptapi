namespace Domain;

public abstract class LargeLanguageModelOptionBase
{
    public required int Index { get; init; }

    public required string? FinishReason { get; init; }
}
