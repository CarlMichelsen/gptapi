using System.Net;
using System.Text.Json;
using BusinessLogic;
using BusinessLogic.Client;
using BusinessLogic.Provider;
using Domain.Configuration;
using Domain.Gpt;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace Test.Client;

public class GptChatClientTests
{
    private readonly GptChatPrompt prompt;
    private readonly GptChatResponse chatResponse;
    private readonly GptChatStreamChunk chatStreamChunk;

    public GptChatClientTests()
    {
        this.prompt = new GptChatPrompt
        {
            Model = "gpt-4",
            Messages = new List<GptChatMessage>
            {
                new GptChatMessage
                {
                    Role = "system",
                    Content = "Only do nice things!",
                },
                new GptChatMessage
                {
                    Role = "user",
                    Content = "Tell me a short story of max 50 words!",
                },
            },
        };

        this.chatResponse = new GptChatResponse
        {
            Id = "I'm an id",
            Object = "I'm an object",
            Created = DateTime.Now.Subtract(TimeSpan.FromMinutes(4)),
            Model = "I'm a model",
            SystemFingerprint = null,
            Choices = new List<GptChoice>
            {
                new GptChoice
                {
                    Index = 0,
                    Message = new GptReceivedMessage
                    {
                        Role = "assistant",
                        Content = "Hello, World!",
                    },
                    FinishReason = "Stop",
                },
            },
        };

        this.chatStreamChunk = new GptChatStreamChunk
        {
            Id = "I'm an id",
            Object = "I'm an object",
            Created = DateTime.Now.Subtract(TimeSpan.FromMinutes(4)),
            Model = "I'm a model",
            SystemFingerprint = null,
            Choices = new List<GptStreamChoice>
            {
                new GptStreamChoice
                {
                    Index = 0,
                    Delta = new GptReceivedMessage
                    {
                        Role = "assistant",
                        Content = "Hello, World!",
                    },
                    FinishReason = "Stop",
                },
            },
        };
    }

    [Fact]
    public async Task Prompt_SendsCorrectHttpRequest()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(this.chatResponse)),
        };

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse)
            .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);
        var gptOptions = this.GetTestOptions("test-api-key");
        var gptApiKeyProvider = new GptApiKeyProvider(gptOptions);
        var logger = new Mock<ILogger<GptChatClient>>();
        
        var client = new GptChatClient(logger.Object, httpClient, gptApiKeyProvider);

        // Act
        var response = await client.Prompt(this.prompt, CancellationToken.None);

        // Assert
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post
                && req.RequestUri!.ToString() == "https://api.openai.com/v1/chat/completions"
                && req.Content!.Headers.ContentType!.ToString() == "application/json; charset=utf-8"),
            ItExpr.IsAny<CancellationToken>());
        Assert.NotNull(response);
    }

    [Fact]
    public async Task StreamPrompt_YieldsCorrectChunks()
    {
        // Arrange
        var responseStream = new MemoryStream();
        var writer = new StreamWriter(responseStream);
        this.chatStreamChunk.Choices.Clear();
        this.chatStreamChunk.Choices.Add(this.GenerateStreamChoice("assistant", "first"));
        writer.Write(JsonSerializer.Serialize(this.chatStreamChunk));

        this.chatStreamChunk.Choices.Clear();
        this.chatStreamChunk.Choices.Add(this.GenerateStreamChoice("assistant", "second"));
        writer.Write(JsonSerializer.Serialize(this.chatStreamChunk));
        writer.Flush();
        responseStream.Position = 0; // Reset stream position to the beginning

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StreamContent(responseStream),
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(handlerMock.Object);
        var gptOptions = this.GetTestOptions("test-api-key");
        var gptApiKeyProvider = new GptApiKeyProvider(gptOptions);
        var logger = new Mock<ILogger<GptChatClient>>();
        
        var client = new GptChatClient(logger.Object, httpClient, gptApiKeyProvider);

        // Act
        var chunks = new List<GptChatStreamChunk>();
        await foreach (var chunk in client.StreamPrompt(this.prompt, CancellationToken.None))
        {
            chunks.Add(chunk);
        }

        // Assert
        Assert.Equal(2, chunks.Count);
        Assert.Equal("first", chunks[0].Choices.First().Delta.Content);
        Assert.Equal("second", chunks[1].Choices.First().Delta.Content);

        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post
                && req.RequestUri!.ToString() == "https://api.openai.com/v1/chat/completions"
                && req.Content!.Headers.ContentType!.ToString() == "application/json; charset=utf-8"),
            ItExpr.IsAny<CancellationToken>());
    }

    private IOptions<GptOptions> GetTestOptions(params string[] apiKeys)
    {
        return Options.Create(new GptOptions { ApiKeys = new List<string>(apiKeys) });
    }

    private GptStreamChoice GenerateStreamChoice(
        string role,
        string content,
        string finnishReason = "Stop")
    {
        return new GptStreamChoice
            {
                Index = 0,
                Delta = new GptReceivedMessage
                {
                    Role = role,
                    Content = content,
                },
                FinishReason = finnishReason,
            };
    }
}