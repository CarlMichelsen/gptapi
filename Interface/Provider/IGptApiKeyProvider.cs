using Domain.Abstractions;
using Domain.LargeLanguageModel.OpenAi;
using Domain.LargeLanguageModel.Shared;

namespace Interface.Provider;

public interface IGptApiKeyProvider
{
    public Task<Result<GptApiKey>> GetReservedApiKey();
}
