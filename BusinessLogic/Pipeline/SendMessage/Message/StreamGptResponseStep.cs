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
    private readonly ILargeLanguageModelClient largeLanguageModelClient;
    private readonly IHubContext<ChatHub, IChatClient> chatHub;

    public StreamGptResponseStep(
        ILogger<StreamGptResponseStep> logger,
        ILargeLanguageModelClient largeLanguageModelClient,
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
            var prompt = LargeLanguageModelMapper.Map(context.Conversation!);
            var gptChunkAsyncEnumerable = this.largeLanguageModelClient.StreamPrompt(prompt, LargeLanguageModelProvider.OpenAi, cancellationToken);

            var orderCounter = 0;
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
                var chunk = this.HandleChunk(orderCounter, gptChunk, context);
                context.MessageChunkDtos.Add(chunk);
                orderCounter++;

                await client.ReceiveMessageChunk(chunk);
            }

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
        ILargeLanguageModelChunkConvertible chunk,
        SendMessagePipelineContext context)
    {
        var llmChunk = chunk.Convert();
        var choice = llmChunk.Options.First();
        var msg = choice.Message;
        return new MessageChunkDto(
            chunkOrderIndex,
            choice.Index,
            context.Conversation!.Id.Value,
            context.AssistantMessage!.Id.Value,
            context.AssistantMessage.PreviousMessage!.Id.Value,
            Enum.GetName(context.AssistantMessage.Role)!.ToLower(),
            msg.Content,
            DateTime.UtcNow);
    }
}
