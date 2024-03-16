namespace Domain.LargeLanguageModel.Claude.Stream;

public class ClaudeGenericStreamEvent
{
    public required ClaudeStreamEventType Type { get; init; }
    
    public required string JsonContent { get; init; }
}
