namespace Domain.LargeLanguageModel.Claude.Stream.Event;

public class EventMessageStop : ClaudeUnknownEventBase, IClaudeEvent
{
    public ClaudeStreamEventType Type => ClaudeStreamEventType.MessageStop;
}
