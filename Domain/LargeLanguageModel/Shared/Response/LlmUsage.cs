namespace Domain.LargeLanguageModel.Shared.Response;

public class LlmUsage
{
    public required int InputTokens { get; init; }
    
    public required int OutputTokens { get; init; }

    public int TotalTokens { get => this.InputTokens + this.OutputTokens; }
}
