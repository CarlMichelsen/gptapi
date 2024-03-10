using Domain.Abstractions;
using Domain.LargeLanguageModel.Shared;
using Domain.LargeLanguageModel.Shared.Interface;

namespace Interface.Client;

public interface ILargeLanguageModelIntegrationClient
{
    IAsyncEnumerable<Result<ILargeLanguageModelChunkConvertible>> StreamPrompt(
        LargeLanguageModelRequest request,
        CancellationToken cancellationToken);
    
    Task<Result<ILargeLanguageModelResponseConvertible>> Prompt(
        LargeLanguageModelRequest request,
        CancellationToken cancellationToken);
}
