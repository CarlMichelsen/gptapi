namespace Domain.LargeLanguageModel.Claude.Stream.Event;

public interface IClaudeEvent
{
    ClaudeStreamEventType Type { get; }
    
    string TypeString { get; }
}
