using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using BusinessLogic.Map.LargeLanguageModel;
using Domain.Abstractions;
using Domain.LargeLanguageModel.Claude;
using Domain.LargeLanguageModel.Shared;
using Domain.LargeLanguageModel.Shared.Interface;
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

    public async Task<Result<ILargeLanguageModelResponseConvertible>> Prompt(
        LargeLanguageModelRequest request,
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
                ClaudeMapper.Map(request, 2000),
                false,
                cancellationToken);
            
            if (responseResult.IsError)
            {
                return responseResult.Error!;
            }

            return await this.MapResponse(responseResult.Unwrap(), cancellationToken);
        }
    }

    public async IAsyncEnumerable<Result<ILargeLanguageModelChunkConvertible>> StreamPrompt(
        LargeLanguageModelRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield break;
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

            var val = response.Content.ReadAsStringAsync();
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

    private async Task<Result<ILargeLanguageModelResponseConvertible>> MapResponse(HttpResponseMessage response, CancellationToken cancellationToken)
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
