using System.Text;

namespace BusinessLogic.Json;

public static class JsonStreamProcessor
{
    public static async IAsyncEnumerable<string> ReadJsonObjectsAsync(Stream stream, int bufferSize = 256)
    {
        var buffer = new byte[bufferSize];
        var jsonBuilder = new StringBuilder();
        int openBraces = 0;
        int quoteCounter = 0;
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            var chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            foreach (var ch in chunk)
            {
                if (IsQuote(ch))
                {
                    quoteCounter++;
                }

                var inQuotes = quoteCounter % 2 == 1;
                var braceDiff = CountCurlyBrackets(ch, inQuotes);
                openBraces += braceDiff;

                if (braceDiff == 1 && openBraces == 1)
                {
                    jsonBuilder.Clear();
                    jsonBuilder.Append(ch);
                }
                else if (braceDiff == -1 && openBraces == 0)
                {
                    jsonBuilder.Append(ch);
                    yield return jsonBuilder.ToString();
                }
                else if (openBraces > 0)
                {
                    jsonBuilder.Append(ch);
                }
            }
        }
    }

    private static bool IsQuote(char character)
    {
        // This is naive because i dont want to count escaped quotes and the escape character may not be part of this chunk...
        return character == '"';
    }

    private static int CountCurlyBrackets(char character, bool inQuotes)
    {
        if (inQuotes)
        {
            return 0;
        }

        if (character == '}')
        {
            return -1;
        }

        if (character == '{')
        {
            return 1;
        }

        return 0;
    }
}
