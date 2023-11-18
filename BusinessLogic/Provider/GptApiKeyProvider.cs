using Domain.Configuration;
using Interface.Provider;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Provider;

public class GptApiKeyProvider : IGptApiKeyProvider
{
    private static readonly List<string> KeysInUse = new();

    private readonly IOptions<GptOptions> gptOptions;

    public GptApiKeyProvider(
        IOptions<GptOptions> gptOptions)
    {
        this.gptOptions = gptOptions;
    }

    public Task<string?> ReserveAKey()
    {
        var availableKeys = this.gptOptions.Value.ApiKeys
            .Where(a => !KeysInUse.Contains(a))
            .ToList();
        
        if (availableKeys.Count == 0)
        {
            return Task.FromResult<string?>(default);
        }

        var key = availableKeys.FirstOrDefault();
        if (key is not null)
        {
            KeysInUse.Add(key);
        }

        return Task.FromResult<string?>(key);
    }

    public Task CancelKeyReservation(string key)
    {
        KeysInUse.Remove(key);
        return Task.CompletedTask;
    }

    public Task UnlockAll()
    {
        KeysInUse.Clear();
        return Task.CompletedTask;
    }
}
