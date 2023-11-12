using Domain.Configuration;
using Microsoft.Extensions.Options;

namespace Test;

public static class TestUtil
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await foreach (var item in source)
        {
            list.Add(item);
        }
        
        return list;
    }

    public static IOptions<GptOptions> CreateGptOptions(List<string> apiKeys)
    {
        var options = Options.Create(new GptOptions { ApiKeys = apiKeys });
        return options;
    }
}
