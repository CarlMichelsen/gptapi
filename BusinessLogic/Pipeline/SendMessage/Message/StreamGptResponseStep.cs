using System.Text.Json;
using BusinessLogic.Hub;
using BusinessLogic.Map;
using Domain.Abstractions;
using Domain.Dto;
using Domain.Dto.Conversation;
using Domain.LargeLanguageModel.OpenAi;
using Domain.LargeLanguageModel.Shared;
using Domain.LargeLanguageModel.Shared.Interface;
using Domain.Pipeline.SendMessage;
using Interface.Client;
using Interface.Hub;
using Interface.Pipeline;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Pipeline.SendMessage.Message;

public class StreamGptResponseStep : IPipelineStep<SendMessagePipelineContext>
{
    private readonly ILogger<StreamGptResponseStep> logger;
    private readonly ILlmClient largeLanguageModelClient;
    private readonly IHubContext<ChatHub, IChatClient> chatHub;

    public StreamGptResponseStep(
        ILogger<StreamGptResponseStep> logger,
        ILlmClient largeLanguageModelClient,
        IHubContext<ChatHub, IChatClient> chatHub)
    {
        this.logger = logger;
        this.largeLanguageModelClient = largeLanguageModelClient;
        this.chatHub = chatHub;
    }

    public async Task<Result<SendMessagePipelineContext>> Execute(
        SendMessagePipelineContext context,
        CancellationToken cancellationToken)
    {
        var client = this.chatHub.Clients.Client(context.ConnectionId);
        if (client is null)
        {
            return new Error(
                "StreamGptResponseStep.NoClient",
                "Unable to access signalR client");
        }

        try
        {
            var systemMsg = "Your persona is a legendary software developer that is very careful when writing code. You always make sure to use the latest version of packages and always double check your code.";
            var prompt = LargeLanguageModelMapper.Map(context.Conversation!, context.LlmModel, systemMsg, context.MaxTokens);
            var gptChunkAsyncEnumerable = this.largeLanguageModelClient.StreamPrompt(prompt, context.LlmProvider, cancellationToken);

            var tokenCounter = 0;
            await foreach (var gptChunkResult in gptChunkAsyncEnumerable)
            {
                if (gptChunkResult.IsError)
                {
                    var err = new ErrorDto(gptChunkResult.Error!);
                    await client.Error(err);
                    break;
                }

                var gptChunk = gptChunkResult.Unwrap();

                cancellationToken.ThrowIfCancellationRequested();
                var chunk = this.HandleChunk(tokenCounter, gptChunk, context);
                context.MessageChunkDtos.Add(chunk);
                tokenCounter++;

                await client.ReceiveMessageChunk(chunk);
            }

            context.TokenUsage = tokenCounter;
            return context;
        }
        catch (OperationCanceledException)
        {
            return new Error(
                "StreamGptResponseStep.Cancelled",
                "Message generation was cancelled");
        }
        catch (Exception e)
        {
            this.logger.LogCritical(
                "GptResponseStream failed with the following exception:\n{exception}",
                e);
            return new Error(
                "StreamGptResponseStep.Exception",
                "Message streaming failed");
        }
    }

    private MessageChunkDto HandleChunk(
        int chunkOrderIndex,
        ILlmChunkConvertible chunk,
        SendMessagePipelineContext context)
    {
        var llmChunk = chunk.Convert();
        var choice = llmChunk.Choices.First();
        return new MessageChunkDto(
            chunkOrderIndex,
            context.Conversation!.Id.Value,
            context.AssistantMessage!.Id.Value,
            context.AssistantMessage.PreviousMessage!.Id.Value,
            Enum.GetName(context.AssistantMessage.Role)!.ToLower(),
            choice.Content,
            DateTime.UtcNow);
    }
}
