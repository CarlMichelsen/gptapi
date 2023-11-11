using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Domain.Configuration;
using Domain.Gpt;
using Interface.Client;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Client;

public class GptChatClient : IGptChatClient
{
    private const string Uri = "https://api.openai.com/v1/chat/completions";
    private readonly HttpClient gptHttpClient;

    public GptChatClient(
        HttpClient gptHttpClient,
        IOptions<GptOptions> gptOptions)
    {
        this.gptHttpClient = gptHttpClient;
        this.gptHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {gptOptions.Value.ApiKey}");
    }

    public async Task<GptChatResponse> Prompt(GptChatPrompt prompt, CancellationToken cancellationToken)
    {
        prompt.Stream = false;

        var body = JsonSerializer.Serialize(prompt);
        var content = new StringContent(body, Encoding.UTF8, "application/json");

        var response = await this.gptHttpClient.PostAsync(Uri, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var resStr = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<GptChatResponse>(resStr)
            ?? throw new JsonException("Failed to deserialize response from OpenAi");
    }

    public async IAsyncEnumerable<GptChatStreamChunk> StreamPrompt(
        GptChatPrompt prompt,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        prompt.Stream = true;

        var body = JsonSerializer.Serialize(prompt);
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, Uri)
        {
            Content = content,
        };
        var response = await this.gptHttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

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
}