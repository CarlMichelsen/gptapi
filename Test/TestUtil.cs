using BusinessLogic.Database;
using Domain.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

    public static ServiceCollection GetServiceCollectionWithDatabase()
    {
        var services = new ServiceCollection();

        services.AddDbContext<ApplicationContext>((options) => 
        {
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
        });

        return services;
    }
}
