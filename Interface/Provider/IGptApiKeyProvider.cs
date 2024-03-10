using Domain.Abstractions;
using Domain.Gpt;

namespace Interface.Provider;

public interface IGptApiKeyProvider
{
    public Task<Result<GptApiKey>> GetReservedApiKey();
}
