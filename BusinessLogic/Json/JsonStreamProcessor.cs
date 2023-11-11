using System.Text;

namespace BusinessLogic;

public static class JsonStreamProcessor
{
    public static async IAsyncEnumerable<string> ReadJsonObjectsAsync(Stream stream, int bufferSize = 256)
    {
        var buffer = new byte[bufferSize];
        var jsonBuilder = new StringBuilder();
        int openBraces = 0;
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            var chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            foreach (var ch in chunk)
            {
                if (ch == '{')
                {
                    if (openBraces == 0)
                    {
                        jsonBuilder.Clear();
                    }

                    openBraces++;
                    jsonBuilder.Append(ch);
                }
                else if (ch == '}')
                {
                    openBraces--;
                    jsonBuilder.Append(ch);

                    if (openBraces == 0 && jsonBuilder.Length > 0)
                    {
                        yield return jsonBuilder.ToString();
                    }
                }
                else if (openBraces > 0)
                {
                    jsonBuilder.Append(ch);
                }
            }
        }
    }
}
