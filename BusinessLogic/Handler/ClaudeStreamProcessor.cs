using System.Text.Json;
using Domain.Abstractions;
using Domain.LargeLanguageModel.Claude.Stream.Event;
using Domain.LargeLanguageModel.Shared.Interface;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler;

/// <summary>
/// This class contains ClaudeStreamEvent handler methods.
/// The methods return:
/// Nullable Result ILlmChunkConvertible.
/// If null is returned: no action is taken.
/// If error result is returned the stream will be cancelled gracefully.
/// If ILlmChunkConvertible result is returned the client will be notified.
/// </summary>
public class ClaudeStreamProcessor
{
    private readonly ILogger logger;
    private readonly List<IClaudeEvent> eventHistory = new();
    private int tokenCount = 0;

    public ClaudeStreamProcessor(ILogger logger)
    {
        this.logger = logger;
    }

    public Result<ILlmChunkConvertible>? HandleMessageStart(EventMessageStart ev)
    {
        this.LogEvent(ev);
        this.tokenCount += ev.Message.Usage.OutputTokens;
        return null;
    }
    
    public Result<ILlmChunkConvertible>? HandleContentBlockStart(EventContentBlockStart ev)
    {
        this.LogEvent(ev);
        return null;
    }

    public Result<ILlmChunkConvertible>? HandlePing(EventPing ev)
    {
        this.LogEvent(ev);
        return null;
    }

    public Result<ILlmChunkConvertible>? HandleContentBlockDelta(EventContentBlockDelta ev)
    {
        this.LogEvent(ev);
        this.tokenCount++;
        return ev;
    }

    public Result<ILlmChunkConvertible>? HandleContentBlockStop(EventContentBlockStop ev)
    {
        this.LogEvent(ev);
        return null;
    }

    public Result<ILlmChunkConvertible>? HandleMessageDelta(EventMessageDelta ev)
    {
        this.LogEvent(ev);
        return null;
    }

    public Result<ILlmChunkConvertible>? HandleMessageStop(EventMessageStop ev)
    {
        this.LogEvent(ev);
        return null;
    }

    private void LogEvent<T>(T ev)
        where T : IClaudeEvent
    {
        this.eventHistory.Add(ev);
        this.logger.LogDebug(
            "EVENT {type}\t{tokenCount}\n{eventJson}",
            Enum.GetName(ev.Type),
            this.tokenCount,
            JsonSerializer.Serialize(ev));
    }
}