using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using BusinessLogic.Json;
using BusinessLogic.Map.LargeLanguageModel;
using Domain.Abstractions;
using Domain.LargeLanguageModel.OpenAi;
using Domain.LargeLanguageModel.Shared;
using Domain.LargeLanguageModel.Shared.Interface;
using Interface.Client;
using Interface.Provider;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Client;

public class GptChatClient : IGptChatClient
{
    // TODO: dont have hardcoded urls.
    private const string Uri = "https://api.openai.com/v1/chat/completions";
    private readonly ILogger<GptChatClient> logger;
    private readonly HttpClient gptHttpClient;
    private readonly IGptApiKeyProvider gptApiKeyProvider;

    public GptChatClient(
        ILogger<GptChatClient> logger,
        HttpClient gptHttpClient,
        IGptApiKeyProvider gptApiKeyProvider)
    {
        this.logger = logger;
        this.gptHttpClient = gptHttpClient;
        this.gptApiKeyProvider = gptApiKeyProvider;
    }

    public async IAsyncEnumerable<Result<ILargeLanguageModelChunkConvertible>> StreamPrompt(
        LargeLanguageModelRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        this.logger.LogInformation("{clientName} was prompted for a stream response", nameof(GptChatClient));
        var keyResult = await this.gptApiKeyProvider.GetReservedApiKey();
        if (keyResult.IsError)
        {
            yield return keyResult.Error!;
            yield break;
        }

        await using (var key = keyResult.Unwrap())
        {
            var httpResponseResult = await this.HandleOpenAiRequest(
                OpenAiMapper.Map(request),
                key,
                true,
                cancellationToken);
            
            if (httpResponseResult.IsError)
            {
                yield return httpResponseResult.Error!;
                yield break;
            }

            var httpRes = httpResponseResult.Unwrap();
            await foreach (var chunk in this.RawStreamPrompt(httpRes, cancellationToken))
            {
                yield return chunk;
            }
        }
    }

    public async Task<Result<ILargeLanguageModelResponseConvertible>> Prompt(
        LargeLanguageModelRequest request,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation("{clientName} was prompted", nameof(GptChatClient));
        var keyResult = await this.gptApiKeyProvider.GetReservedApiKey();
        if (keyResult.IsError)
        {
            return keyResult.Error!;
        }

        await using (var key = keyResult.Unwrap())
        {
            var httpResponseResult = await this.HandleOpenAiRequest(
                OpenAiMapper.Map(request),
                key,
                false,
                cancellationToken);
            
            if (httpResponseResult.IsError)
            {
                return httpResponseResult.Error!;
            }

            var httpRes = httpResponseResult.Unwrap();
            return await this.RawPrompt(httpRes, cancellationToken);
        }
    }

    private async Task<Result<HttpResponseMessage>> HandleOpenAiRequest(
        GptChatPrompt prompt,
        GptApiKey gptApiKey,
        bool isStream,
        CancellationToken cancellationToken)
    {
        try
        {
            return await this.OpenAiRequest(prompt, gptApiKey, isStream, cancellationToken);
        }
        catch (HttpRequestException e)
            when (e.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            this.logger.LogCritical("Request to OpenAi with apikey was not authorized {e}", e);
            return new Error("Prompt.Unauthorized", e.Message);
        }
        catch (HttpRequestException e)
        {
            this.logger.LogCritical("Request to OpenAi with apikey failed {e}", e);
            return new Error("Prompt.HttpRequestException", "Internal HTTP related error");
        }
        catch (Exception e)
        {
            this.logger.LogCritical("Request to OpenAi with apikey failed because of a critical error {e}", e);
            return new Error("Prompt.Exception", "Internal Server Error");
        }
    }

    private async Task<HttpResponseMessage> OpenAiRequest(
        GptChatPrompt prompt,
        GptApiKey key,
        bool isStream,
        CancellationToken cancellationToken)
    {
        prompt.Stream = isStream;
        var body = JsonSerializer.Serialize(prompt);

        var content = new StringContent(body, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, Uri)
        {
            Content = content,
            Headers = 
            {
                { "Authorization", $"Bearer {key.ApiKey}" },
            },
        };
        var response = await this.gptHttpClient.SendAsync(
            request,
            isStream ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        return response;
    }

    private async IAsyncEnumerable<GptChatStreamChunk> RawStreamPrompt(
        HttpResponseMessage response,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var stream = response.Content.ReadAsStream(cancellationToken);
        await foreach (var chunk in GptResponseStreamProcessor.ReadGptStream(stream))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(chunk))
            {
                continue;
            }

            var deserializedChunk = JsonSerializer.Deserialize<GptChatStreamChunk>(chunk);
            if (deserializedChunk is null)
            {
                continue;
            }

            yield return deserializedChunk;
        }
    }

    private async Task<Result<ILargeLanguageModelResponseConvertible>> RawPrompt(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var resStr = await response.Content.ReadAsStringAsync(cancellationToken);

        try
        {
            var res = JsonSerializer.Deserialize<GptChatResponse>(resStr);
            if (res is null)
            {
                return new Error("RawPrompt.DeserialisationResultedInNullValue");
            }

            return res;
        }
        catch (System.Exception)
        {
            var err = new Error("RawPrompt.FailedToDeserializePrompt");
            this.logger.LogWarning("{code}\n{content}", err.Code, resStr);
            return err;
        }
    }
}