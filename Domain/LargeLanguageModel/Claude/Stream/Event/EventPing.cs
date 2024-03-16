namespace Domain.LargeLanguageModel.Claude.Stream.Event;

public class EventPing : ClaudeUnknownEventBase, IClaudeEvent
{
    public ClaudeStreamEventType Type => ClaudeStreamEventType.Ping;
}
