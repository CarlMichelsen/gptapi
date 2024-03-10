using Domain.Abstractions;
using Domain.Configuration;
using Domain.Gpt;
using Interface.Provider;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Provider;

public class GptApiKeyProvider : IGptApiKeyProvider
{
    private static readonly Random RandomObject = new();
    private static readonly List<string> KeysInUse = new();

    private readonly IOptions<GptOptions> gptOptions;

    public GptApiKeyProvider(
        IOptions<GptOptions> gptOptions)
    {
        this.gptOptions = gptOptions;
    }

    public async Task<Result<GptApiKey>> GetReservedApiKey()
    {
        var availableKeys = this.GetAvailableKeys();

        if (availableKeys.Count == 0)
        {
            return new Error("ReserveApiKey.NoAvailableKeys");
        }

        var apiKeyString = this.GetRandomElement(availableKeys, RandomObject);
        if (string.IsNullOrWhiteSpace(apiKeyString))
        {
            return new Error("ReserveApiKey.ReservedApiKeyStringIsNullOrWhitespace");
        }

        if (KeysInUse.Contains(apiKeyString))
        {
            return new Error("ReserveApiKey.ApiKeyAlreadyReserved");
        }
        else
        {
            KeysInUse.Add(apiKeyString);
        }
        
        return await Task.Run(() => new GptApiKey(apiKeyString, (apiKey) => this.CancelKeyReservation(apiKey)));
    }

    private List<string> GetAvailableKeys()
    {
        return this.gptOptions.Value.ApiKeys
            .Where(a => !KeysInUse.Contains(a))
            .ToList();
    }

    private T GetRandomElement<T>(List<T> list, Random? rand = default)
    {
        var random = rand ?? new Random();
        var index = random.Next(list.Count);

        return list[index];
    }

    private Task CancelKeyReservation(GptApiKey apiKey)
    {
        if (apiKey?.ApiKey is not null)
        {
            KeysInUse.Remove(apiKey.ApiKey);
        }
        
        return Task.CompletedTask;
    }
}


/*
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
*/