using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using BusinessLogic.Json;
using Domain.Abstractions;
using Domain.Gpt;
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

    public async Task<Result<GptChatResponse>> Prompt(GptChatPrompt prompt, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("{clientName} was prompted", nameof(GptChatClient));
        var key = await this.gptApiKeyProvider.ReserveAKey();
        
        if (key is null)
        {
            this.logger.LogWarning("No apikey was available");
            return new Error("Prompt.ApiKey", "No apikey was available");
        }

        try
        {
            var httpRes = await this.OpenAiRequest(prompt, key, false, cancellationToken);
            return await this.RawPrompt(httpRes, cancellationToken);
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
        finally
        {
            await this.gptApiKeyProvider.CancelKeyReservation(key);
        }
    }

    public async IAsyncEnumerable<GptChatStreamChunk> StreamPrompt(
        GptChatPrompt prompt,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        this.logger.LogInformation("{clientName} was prompted for a stream response", nameof(GptChatClient));
        var key = await this.gptApiKeyProvider.ReserveAKey();
        if (key is null)
        {
            this.logger.LogWarning("No apikey was available");
            yield break;
        }

        HttpResponseMessage? httpRes = default;
        try
        {
            httpRes = await this.OpenAiRequest(prompt, key, true, cancellationToken);
        }
        catch (HttpRequestException e)
            when (e.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            this.logger.LogCritical("Request to OpenAi with apikey was not authorized {e}", e);
        }
        catch (HttpRequestException e)
        {
            this.logger.LogWarning("Request to OpenAi with apikey failed {e}", e);
        }
        catch (Exception e)
        {
            this.logger.LogCritical("Request to OpenAi with apikey failed because of a critical error {e}", e);
        }

        if (httpRes is null)
        {
            await this.gptApiKeyProvider.CancelKeyReservation(key);
            yield break;
        }

        await foreach (var chunk in this.RawStreamPrompt(httpRes, cancellationToken))
        {
            yield return chunk;
        }

        await this.gptApiKeyProvider.CancelKeyReservation(key);
    }

    private async Task<HttpResponseMessage> OpenAiRequest(
        GptChatPrompt prompt,
        string key,
        bool isStream,
        CancellationToken cancellationToken)
    {
        prompt.Stream = isStream;
        var body = JsonSerializer.Serialize(prompt);

        this.logger.LogCritical(body);

        var content = new StringContent(body, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, Uri)
        {
            Content = content,
            Headers = 
            {
                { "Authorization", $"Bearer {key}" },
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
        await foreach (var chunk in JsonStreamProcessor.ReadJsonObjectsAsync(stream))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var deserializedChunk = JsonSerializer.Deserialize<GptChatStreamChunk>(chunk);
            if (deserializedChunk is null)
            {
                continue;
            }

            yield return deserializedChunk;
        }
    }

    private async Task<GptChatResponse> RawPrompt(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var resStr = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<GptChatResponse>(resStr)
            ?? throw new JsonException("Failed to deserialize response from OpenAi");
    }
}