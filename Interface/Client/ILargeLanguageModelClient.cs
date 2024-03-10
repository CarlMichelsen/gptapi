using Domain.Abstractions;
using Domain.LargeLanguageModel.Shared;
using Domain.LargeLanguageModel.Shared.Interface;

namespace Interface.Client;

public interface ILargeLanguageModelClient
{
    IAsyncEnumerable<Result<ILargeLanguageModelChunkConvertible>> StreamPrompt(
        LargeLanguageModelRequest request,
        LargeLanguageModelProvider provider,
        CancellationToken cancellationToken);
    
    Task<Result<ILargeLanguageModelResponseConvertible>> Prompt(
        LargeLanguageModelRequest request,
        LargeLanguageModelProvider provider,
        CancellationToken cancellationToken);
}