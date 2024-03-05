using System.Text;
using BusinessLogic.Hub;
using BusinessLogic.Map;
using Database;
using Domain.Abstractions;
using Domain.Dto.Conversation;
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

        var content = this.ResponseContent(context.MessageChunkDtos);
        if (string.IsNullOrWhiteSpace(content))
        {
            return new Error(
                "CompleteGptResponseMessageStep.IncompleteMessage",
                "Message content empty");
        }

        context.AssistantMessage!.Content = content;
        context.AssistantMessage!.CompletedUtc = DateTime.UtcNow;
        context.Conversation!.Messages.Add(context.AssistantMessage!);

        var rsvMsgDto = new ReceiveMessageDto
        {
            ConversationId = context.Conversation.Id.Value,
            Message = ConversationMapper.Map(context.AssistantMessage!),
        };
        await client.ReceiveMessage(rsvMsgDto);

        context.Conversation.LastAppendedUtc = DateTime.UtcNow;

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
