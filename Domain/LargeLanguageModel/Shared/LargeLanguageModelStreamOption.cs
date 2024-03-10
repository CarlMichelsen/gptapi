namespace Domain.LargeLanguageModel.Shared;

public class LargeLanguageModelStreamOption : LargeLanguageModelOptionBase
{
    public required LargeLanguageModelDelta Message { get; init; }
}
