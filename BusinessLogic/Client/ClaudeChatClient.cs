using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using BusinessLogic.Handler;
using BusinessLogic.Json;
using BusinessLogic.Map.LargeLanguageModel;
using Domain.Abstractions;
using Domain.LargeLanguageModel.Claude;
using Domain.LargeLanguageModel.Claude.Stream;
using Domain.LargeLanguageModel.Claude.Stream.Event;
using Domain.LargeLanguageModel.Shared.Interface;
using Domain.LargeLanguageModel.Shared.Request;
using Interface.Client;
using Interface.Provider;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Client;

public class ClaudeChatClient : IClaudeChatClient
{
    // TODO: dont have hardcoded urls.
    private const string Uri = "https://api.anthropic.com/v1/messages";

    private readonly ILogger<ClaudeChatClient> logger;
    private readonly HttpClient gptHttpClient;
    private readonly IClaudeApiKeyProvider claudeApiKeyProvider;

    public ClaudeChatClient(
        ILogger<ClaudeChatClient> logger,
        HttpClient gptHttpClient,
        IClaudeApiKeyProvider claudeApiKeyProvider)
    {
        this.logger = logger;
        this.gptHttpClient = gptHttpClient;
        this.claudeApiKeyProvider = claudeApiKeyProvider;
    }

    public async Task<Result<ILlmResponseConvertible>> Prompt(
        LlmRequest request,
        CancellationToken cancellationToken)
    {
        var apiKeyResult = await this.claudeApiKeyProvider.GetReservedApiKey();
        if (apiKeyResult.IsError)
        {
            return apiKeyResult.Error!;
        }

        await using (var key = apiKeyResult.Unwrap())
        {
            var responseResult = await this.PromptClaude(
                key,
                ClaudeMapper.Map(request, request.MaxTokens),
                false,
                cancellationToken);
            
            if (responseResult.IsError)
            {
                return responseResult.Error!;
            }

            return await this.MapResponse(responseResult.Unwrap(), cancellationToken);
        }
    }

    public async IAsyncEnumerable<Result<ILlmChunkConvertible>> StreamPrompt(
        LlmRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var apiKeyResult = await this.claudeApiKeyProvider.GetReservedApiKey();
        if (apiKeyResult.IsError)
        {
            yield return apiKeyResult.Error!;
            yield break;
        }

        await using (var key = apiKeyResult.Unwrap())
        {
            var responseResult = await this.PromptClaude(
                key,
                ClaudeMapper.Map(request, request.MaxTokens),
                true,
                cancellationToken);
            
            if (responseResult.IsError)
            {
                yield return responseResult.Error!;
                yield break;
            }

            var httpResponseStream = responseResult
                .Unwrap().Content
                .ReadAsStream(cancellationToken);

            var handler = new ClaudeStreamProcessor(this.logger);
            await foreach (var chunkResult in ClaudeResponseStreamProcessor.ReadClaudeStream(httpResponseStream))
            {
                if (chunkResult.IsError)
                {
                    yield return chunkResult.Error!;
                    yield break;
                }

                var chunk = chunkResult.Unwrap();
                Result<ILlmChunkConvertible>? res = null;

                switch (chunk.Type)
                {
                    case ClaudeStreamEventType.MessageStart:
                        res = this.HandleEvent<EventMessageStart>(chunk.JsonContent, handler.HandleMessageStart);
                        break;
                    
                    case ClaudeStreamEventType.ContentBlockStart:
                        res = this.HandleEvent<EventContentBlockStart>(chunk.JsonContent, handler.HandleContentBlockStart);
                        break;
                    
                    case ClaudeStreamEventType.Ping:
                        res = this.HandleEvent<EventPing>(chunk.JsonContent, handler.HandlePing);
                        break;

                    case ClaudeStreamEventType.ContentBlockDelta:
                        res = this.HandleEvent<EventContentBlockDelta>(chunk.JsonContent, handler.HandleContentBlockDelta);
                        break;
                    
                    case ClaudeStreamEventType.ContentBlockStop:
                        res = this.HandleEvent<EventContentBlockStop>(chunk.JsonContent, handler.HandleContentBlockStop);
                        break;
                    
                    case ClaudeStreamEventType.MessageDelta:
                        res = this.HandleEvent<EventMessageDelta>(chunk.JsonContent, handler.HandleMessageDelta);
                        break;
                    
                    case ClaudeStreamEventType.MessageStop:
                        res = this.HandleEvent<EventMessageStop>(chunk.JsonContent, handler.HandleMessageStop);
                        break;

                    default:
                        yield return new Error("Unhandled ClaudeStreamEvent Type");
                        yield break;
                }

                if (res is not null)
                {
                    yield return res;
                    if (res.IsError)
                    {
                        yield break;
                    }
                }
            }
        }
    }

    private Result<ILlmChunkConvertible>? HandleEvent<T>(
        string jsonContent,
        Func<T, Result<ILlmChunkConvertible>?> action)
        where T : IClaudeEvent
    {
        var deserialized = JsonSerializer.Deserialize<T>(jsonContent);
        if (deserialized is null)
        {
            return new Error("Unable to deserialize ClaudeStreamEventType.ContentBlockDelta");
        }

        return action(deserialized);
    }

    private async Task<Result<HttpResponseMessage>> PromptClaude(
        ClaudeApiKey claudeApiKey,
        ClaudePrompt claudePrompt,
        bool isStream,
        CancellationToken cancellationToken)
    {
        try
        {
            claudePrompt.Stream = isStream;
            var jsonPrompt = JsonSerializer.Serialize(claudePrompt);

            var content = new StringContent(jsonPrompt, Encoding.UTF8, "application/json");
            using var request = new HttpRequestMessage(HttpMethod.Post, Uri)
            {
                Content = content,
                Headers = 
                {
                    { "anthropic-version", "2023-06-01" },
                    { "anthropic-beta", "messages-2023-12-15" },
                    { "x-api-key", claudeApiKey.ApiKey },
                },
            };
            var response = await this.gptHttpClient.SendAsync(
                request,
                isStream ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead,
                cancellationToken);

            response.EnsureSuccessStatusCode();
            return response;
        }
        catch (HttpRequestException e)
        {
            return new Error("ClaudeChatClient.HttpRequestException", e.StatusCode.ToString());
        }
        catch (Exception e)
        {
            return new Error("ClaudeChatClient.Exception", e.Message);
        }
    }

    private async Task<Result<ILlmResponseConvertible>> MapResponse(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var resStr = await response.Content.ReadAsStringAsync(cancellationToken);

        try
        {
            var res = JsonSerializer.Deserialize<ClaudeResponse>(resStr);
            if (res is null)
            {
                return new Error("ClaudeChatClient.DeserialisationResultedInNullValue");
            }

            return res;
        }
        catch (Exception e)
        {
            var err = new Error("ClaudeChatClient.FailedToDeserializePrompt");
            this.logger.LogWarning("{code}\n{exception}\n{content}", err.Code, e, resStr);
            return err;
        }
    }
}
