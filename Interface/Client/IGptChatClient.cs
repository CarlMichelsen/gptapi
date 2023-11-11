using Domain.Gpt;

namespace Interface.Client;

public interface IGptChatClient
{
    IAsyncEnumerable<GptChatStreamChunk> StreamPrompt(
        GptChatPrompt prompt,
        CancellationToken cancellationToken);
    
    Task<GptChatResponse> Prompt(
        GptChatPrompt prompt,
        CancellationToken cancellationToken);
}