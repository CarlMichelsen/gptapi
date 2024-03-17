using Domain.Abstractions;
using Domain.LargeLanguageModel.Claude;

namespace Interface.Provider;

public interface IClaudeApiKeyProvider
{
    public Task<Result<ClaudeApiKey>> GetReservedApiKey();
}
