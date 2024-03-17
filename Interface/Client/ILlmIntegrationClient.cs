using Domain.Abstractions;
using Domain.LargeLanguageModel.Shared.Interface;
using Domain.LargeLanguageModel.Shared.Request;

namespace Interface.Client;

public interface ILlmIntegrationClient
{
    IAsyncEnumerable<Result<ILlmChunkConvertible>> StreamPrompt(
        LlmRequest request,
        CancellationToken cancellationToken);
    
    Task<Result<ILlmResponseConvertible>> Prompt(
        LlmRequest request,
        CancellationToken cancellationToken);
}
