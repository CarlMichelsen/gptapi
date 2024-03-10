namespace Domain.LargeLanguageModel.Shared;

public class LargeLanguageModelResponse : LargeLanguageModelResponseBase
{
    public required List<LargeLanguageModelOption> Options { get; init; }
}
