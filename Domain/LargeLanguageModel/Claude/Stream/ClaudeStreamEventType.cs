namespace Domain.LargeLanguageModel.Claude.Stream;

public enum ClaudeStreamEventType
{
    /// <summary>
    /// message_start.
    /// </summary>
    MessageStart,

    /// <summary>
    /// content_block_start.
    /// </summary>
    ContentBlockStart,

    /// <summary>
    /// ping.
    /// </summary>
    Ping,

    /// <summary>
    /// content_block_delta.
    /// </summary>
    ContentBlockDelta,

    /// <summary>
    /// message_delta.
    /// </summary>
    MessageDelta,

    /// <summary>
    /// message_stop.
    /// </summary>
    MessageStop,
}
