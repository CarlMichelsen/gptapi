using System.Collections.Immutable;
using System.Text;
using Domain.Abstractions;
using Domain.LargeLanguageModel.Claude.Stream;

namespace BusinessLogic.Json;

public static class ClaudeResponseStreamProcessor
{
    public static readonly ImmutableDictionary<string, ClaudeStreamEventType> EventMap = ImmutableDictionary.CreateRange(
        new KeyValuePair<string, ClaudeStreamEventType>[]
        {
            KeyValuePair.Create("message_start",        ClaudeStreamEventType.MessageStart),
            KeyValuePair.Create("content_block_start",  ClaudeStreamEventType.ContentBlockStart),
            KeyValuePair.Create("ping",                 ClaudeStreamEventType.Ping),
            KeyValuePair.Create("content_block_delta",  ClaudeStreamEventType.ContentBlockDelta),
            KeyValuePair.Create("content_block_stop",   ClaudeStreamEventType.ContentBlockStop),
            KeyValuePair.Create("message_delta",        ClaudeStreamEventType.MessageDelta),
            KeyValuePair.Create("message_stop",         ClaudeStreamEventType.MessageStop),
        });
    
    private const string EventLineStart = "event: ";
    private const string DataLineStart = "data: ";

    public static async IAsyncEnumerable<Result<ClaudeGenericStreamEvent>> ReadClaudeStream(Stream stream)
    {
        using StreamReader sr = new StreamReader(stream, Encoding.UTF8);
        
        string? eventString = default;
        while (!sr.EndOfStream)
        {
            var line = await sr.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
            {
                eventString = default;
                continue;
            }

            if (line.StartsWith(EventLineStart))
            {
                eventString = line.Split(EventLineStart).Skip(1).First();
                continue;
            }
            
            if (line.StartsWith(DataLineStart))
            {
                var dataResult = HandleData(stream, eventString, line);
                if (dataResult.IsError)
                {
                    yield return dataResult;
                    yield break;
                }

                yield return dataResult;
            }
        }
    }

    private static Result<ClaudeGenericStreamEvent> HandleData(Stream stream, string? eventString, string line)
    {
        if (string.IsNullOrWhiteSpace(eventString))
        {
            stream.Close();
            return new Error("ClaudeResponseStreamProcessor.DataCameBeforeEventType");
        }

        if (EventMap.TryGetValue(eventString, out var streamEvent))
        {
            var jsonString = line.Split(DataLineStart).Skip(1).First();
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                stream.Close();
                return new Error("ClaudeResponseStreamProcessor.ReceievedEmptyData");
            }

            return new ClaudeGenericStreamEvent
            {
                Type = streamEvent,
                JsonContent = jsonString,
            };
        }
        
        stream.Close();
        return new Error("ClaudeResponseStreamProcessor.NoClaudeStreamEventType");
    }
}
