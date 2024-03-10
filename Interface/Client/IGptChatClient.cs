using Domain.Abstractions;
using Domain.Gpt;

namespace Interface.Client;

public interface IGptChatClient
{
    IAsyncEnumerable<Result<GptChatStreamChunk>> StreamPrompt(
        GptChatPrompt prompt,
        CancellationToken cancellationToken);
    
    Task<Result<GptChatResponse>> Prompt(
        GptChatPrompt prompt,
        CancellationToken cancellationToken);
}