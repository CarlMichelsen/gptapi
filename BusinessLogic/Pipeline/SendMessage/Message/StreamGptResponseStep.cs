using BusinessLogic.Hub;
using BusinessLogic.Map;
using Domain.Abstractions;
using Domain.Dto.Conversation;
using Domain.Gpt;
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
    private readonly IGptChatClient gptChatClient;
    private readonly IHubContext<ChatHub, IChatClient> chatHub;

    public StreamGptResponseStep(
        ILogger<StreamGptResponseStep> logger,
        IGptChatClient gptChatClient,
        IHubContext<ChatHub, IChatClient> chatHub)
    {
        this.logger = logger;
        this.gptChatClient = gptChatClient;
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
            var prompt = GptMapper.Map(context.Conversation!);
            var gptChunkAsyncEnumerable = this.gptChatClient.StreamPrompt(prompt, cancellationToken);

            var orderCounter = 0;
            await foreach (var gptChunk in gptChunkAsyncEnumerable)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var chunk = this.HandleChunk(orderCounter, gptChunk, context);
                context.MessageChunkDtos.Add(chunk);
                orderCounter++;

                await client.ReceiveMessageChunk(chunk);
            }
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

        throw new NotImplementedException();
    }

    private MessageChunkDto HandleChunk(
        int chunkOrderIndex,
        GptChatStreamChunk gptChunk,
        SendMessagePipelineContext context)
    {
        var choice = gptChunk.Choices.First();
        return new MessageChunkDto(
            chunkOrderIndex,
            choice.Index,
            context.Conversation!.Id.Value,
            context.AssistantMessage!.Id.Value,
            context.AssistantMessage.PreviousMessage!.Id.Value,
            Enum.GetName(context.AssistantMessage.Role)!.ToLower(),
            choice.Delta.Content,
            DateTime.UtcNow);
    }
}
