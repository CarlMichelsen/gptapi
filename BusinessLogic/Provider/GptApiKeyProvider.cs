using Domain.Abstractions;
using Domain.Configuration;
using Domain.LargeLanguageModel.OpenAi;
using Domain.LargeLanguageModel.Shared;
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
            return new Error("GptApiKeyProvider.NoAvailableKeys");
        }

        var apiKeyString = this.GetRandomElement(availableKeys, RandomObject);
        if (string.IsNullOrWhiteSpace(apiKeyString))
        {
            return new Error("GptApiKeyProvider.ReservedApiKeyStringIsNullOrWhitespace");
        }

        if (KeysInUse.Contains(apiKeyString))
        {
            return new Error("GptApiKeyProvider.ApiKeyAlreadyReserved");
        }
        else
        {
            KeysInUse.Add(apiKeyString);
        }
        
        return await Task.Run(() => new GptApiKey(apiKeyString, this.CancelKeyReservation));
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

    private Task CancelKeyReservation(LlmReservableApiKey apiKey)
    {
        if (apiKey?.ApiKey is not null)
        {
            KeysInUse.Remove(apiKey.ApiKey);
        }
        
        return Task.CompletedTask;
    }
}