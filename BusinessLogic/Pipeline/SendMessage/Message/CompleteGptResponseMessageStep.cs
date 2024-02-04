using System.Text;
using BusinessLogic.Hub;
using BusinessLogic.Map;
using Database;
using Domain.Abstractions;
using Domain.Dto.Conversation;
using Domain.Entity;
using Domain.Exception;
using Domain.Pipeline.SendMessage;
using Interface.Hub;
using Interface.Pipeline;
using Microsoft.AspNetCore.SignalR;

namespace BusinessLogic.Pipeline.SendMessage.Message;

public class CompleteGptResponseMessageStep : IPipelineStep<SendMessagePipelineContext>
{
    private readonly ApplicationContext applicationContext;
    private readonly IHubContext<ChatHub, IChatClient> chatHub;

    public CompleteGptResponseMessageStep(
        ApplicationContext applicationContext,
        IHubContext<ChatHub, IChatClient> chatHub)
    {
        this.applicationContext = applicationContext;
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
                "CompleteGptResponseMessageStep.NoClient",
                "Unable to access signalR client");
        }

        context.AssistantMessage!.Content = this.ResponseContent(context.MessageChunkDtos);
        context.AssistantMessage!.CompletedUtc = DateTime.UtcNow;
        context.Conversation!.Messages.Add(context.AssistantMessage!);

        await client.ReceiveMessage(
            ConversationMapper.Map(context.AssistantMessage!));

        await this.applicationContext.SaveChangesAsync();
        return context;
    }

    private string ResponseContent(List<MessageChunkDto> messageChunkDtos)
    {
        return messageChunkDtos
            .OrderBy(mc => mc.ChunkOrderIndex)
            .Aggregate(new StringBuilder(), (sb, chunk) => sb.Append(chunk.Content))
            .ToString();
    }
}
