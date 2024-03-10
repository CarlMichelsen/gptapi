namespace Domain.LargeLanguageModel.Shared;

public class LargeLanguageModelChunk : LargeLanguageModelResponseBase
{
    public required List<LargeLanguageModelStreamOption> Options { get; init; }
}
