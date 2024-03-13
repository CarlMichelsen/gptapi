using Domain.Abstractions;
using Domain.Entity;
using Domain.LargeLanguageModel.Shared.Interface;
using Domain.LargeLanguageModel.Shared.Request;

namespace Interface.Client;

public interface ILlmClient
{
    IAsyncEnumerable<Result<ILlmChunkConvertible>> StreamPrompt(
        LlmRequest request,
        LlmProvider provider,
        CancellationToken cancellationToken);
    
    Task<Result<ILlmResponseConvertible>> Prompt(
        LlmRequest request,
        LlmProvider provider,
        CancellationToken cancellationToken);
}