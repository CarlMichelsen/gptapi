﻿using System.Text;

namespace BusinessLogic.Json;

public static class GptResponseStreamProcessor
{
    public const string LineStartDelimeter = "data: ";
    public const string DoneText = "[DONE]";

    public static async IAsyncEnumerable<string> ReadGptStream(Stream stream)
    {
        using StreamReader sr = new StreamReader(stream, Encoding.UTF8);
        
        while (!sr.EndOfStream)
        {
            var line = await sr.ReadLineAsync();
            if (line?.StartsWith(LineStartDelimeter) != true)
            {
                continue;
            }

            var str = line?.Split("data: ").Skip(1).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(str))
            {
                continue;
            }

            if (str == DoneText)
            {
                break;
            }

            yield return str;
        }
    }
}