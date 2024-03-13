namespace Domain.LargeLanguageModel.Claude.Stream;

public class ClaudeStreamEvent
{
    public required ClaudeStreamEventType Type { get; init; }
    
    public required string JsonContent { get; init; }
}
