﻿using System.Runtime.CompilerServices;
using Domain.Abstractions;
using Domain.Entity;
using Domain.LargeLanguageModel.Shared.Interface;
using Domain.LargeLanguageModel.Shared.Request;
using Interface.Client;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic.Client;

public class LargeLanguageModelClient : ILlmClient
{
    private readonly IServiceProvider serviceProvider;

    public LargeLanguageModelClient(
        IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task<Result<ILlmResponseConvertible>> Prompt(
        LlmRequest request,
        LlmProvider provider,
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

    public async IAsyncEnumerable<Result<ILlmChunkConvertible>> StreamPrompt(
        LlmRequest request,
        LlmProvider provider,
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

    private Result<ILlmIntegrationClient> CreateLargeLanguageModelClient(LlmProvider provider)
    {
        return provider switch {
            LlmProvider.OpenAi => this.CreateClient<IGptChatClient>(),
            LlmProvider.Anthropic => this.CreateClient<IClaudeChatClient>(),
            _ => new Error("CreateLargeLanguageModelClient.DidNotFindImplementedClient", $"Attempted to instantiate {nameof(provider)}"),
        };
    }

    private Result<ILlmIntegrationClient> CreateClient<T>()
        where T : notnull
    {
        var service = this.serviceProvider.GetRequiredService<T>() as ILlmIntegrationClient;
        if (service is null)
        {
            return new Error("CreateClient.FailedToCreateClient");
        }

        return new Result<ILlmIntegrationClient>(service);
    }
}
