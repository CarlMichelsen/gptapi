using System.Runtime.CompilerServices;
using Domain.Abstractions;
using Domain.LargeLanguageModel.Shared;
using Domain.LargeLanguageModel.Shared.Interface;
using Interface.Client;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic.Client;

public class LargeLanguageModelClient : ILargeLanguageModelClient
{
    private readonly IServiceProvider serviceProvider;

    public LargeLanguageModelClient(
        IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task<Result<ILargeLanguageModelResponseConvertible>> Prompt(
        LargeLanguageModelRequest request,
        LargeLanguageModelProvider provider,
        CancellationToken cancellationToken)
    {
        var clientResult = this.CreateLargeLanguageModelClient(provider);
        if (clientResult.IsError)
        {
            return clientResult.Error!;
        }
        
        var client = clientResult.Unwrap();
        return await client.Prompt(request, cancellationToken);
    }

    public async IAsyncEnumerable<Result<ILargeLanguageModelChunkConvertible>> StreamPrompt(
        LargeLanguageModelRequest request,
        LargeLanguageModelProvider provider,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var clientResult = this.CreateLargeLanguageModelClient(provider);
        if (clientResult.IsError)
        {
            yield return clientResult.Error!;
            yield break;
        }
        
        var client = clientResult.Unwrap();
        var streamPromptEnumerable = client.StreamPrompt(request, cancellationToken);
        await foreach (var convertible in streamPromptEnumerable)
        {
            yield return convertible;
        }
    }

    private Result<ILargeLanguageModelIntegrationClient> CreateLargeLanguageModelClient(LargeLanguageModelProvider provider)
    {
        return provider switch {
            LargeLanguageModelProvider.OpenAi => this.CreateClient<IGptChatClient>(),
            LargeLanguageModelProvider.Claude => this.CreateClient<IClaudeChatClient>(),
            _ => new Error("CreateLargeLanguageModelClient.DidNotFindImplementedClient", $"Attempted to instantiate {nameof(provider)}"),
        };
    }

    private Result<ILargeLanguageModelIntegrationClient> CreateClient<T>()
        where T : notnull
    {
        var service = this.serviceProvider.GetRequiredService<T>() as ILargeLanguageModelIntegrationClient;
        if (service is null)
        {
            return new Error("CreateClient.FailedToCreateClient");
        }

        return new Result<ILargeLanguageModelIntegrationClient>(service);
    }
}
