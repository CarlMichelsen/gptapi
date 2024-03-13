using Domain.Abstractions;
using Domain.Configuration;
using Domain.LargeLanguageModel.Claude;
using Domain.LargeLanguageModel.Shared;
using Interface.Provider;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Provider;

public class ClaudeApiKeyProvider : IClaudeApiKeyProvider
{
    private static readonly Random RandomObject = new();
    private static readonly List<string> KeysInUse = new();

    private readonly IOptions<ClaudeOptions> claudeOptions;

    public ClaudeApiKeyProvider(
        IOptions<ClaudeOptions> claudeOptions)
    {
        this.claudeOptions = claudeOptions;
    }

    public async Task<Result<ClaudeApiKey>> GetReservedApiKey()
    {
        var availableKeys = this.GetAvailableKeys();

        if (availableKeys.Count == 0)
        {
            return new Error("ClaudeApiKeyProvider.NoAvailableKeys");
        }

        var apiKeyString = this.GetRandomElement(availableKeys, RandomObject);
        if (string.IsNullOrWhiteSpace(apiKeyString))
        {
            return new Error("ClaudeApiKeyProvider.ReservedApiKeyStringIsNullOrWhitespace");
        }

        if (KeysInUse.Contains(apiKeyString))
        {
            return new Error("ClaudeApiKeyProvider.ApiKeyAlreadyReserved");
        }
        else
        {
            KeysInUse.Add(apiKeyString);
        }
        
        return await Task.Run(() => new ClaudeApiKey(apiKeyString, this.CancelKeyReservation));
    }

    private List<string> GetAvailableKeys()
    {
        return this.claudeOptions.Value.ApiKeys
            .Where(a => !KeysInUse.Contains(a))
            .ToList();
    }

    private T GetRandomElement<T>(List<T> list, Random? rand = default)
    {
        var random = rand ?? new Random();
        var index = random.Next(list.Count);

        return list[index];
    }

    private Task CancelKeyReservation(LargeLanguageModelReservableApiKey apiKey)
    {
        if (apiKey?.ApiKey is not null)
        {
            KeysInUse.Remove(apiKey.ApiKey);
        }
        
        return Task.CompletedTask;
    }
}
