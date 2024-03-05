using BusinessLogic.Hub;
using BusinessLogic.Map;
using Database;
using Domain.Abstractions;
using Domain.Dto.Conversation;
using Domain.Entity.Id;
using Domain.Pipeline.SendMessage;
using Interface.Hub;
using Interface.Pipeline;
using Microsoft.AspNetCore.SignalR;

namespace BusinessLogic.Pipeline.SendMessage.Message;

public class InitiateNewMessageStep : IPipelineStep<SendMessagePipelineContext>
{
    private readonly ApplicationContext applicationContext;
    private readonly IHubContext<ChatHub, IChatClient> chatHub;

    public InitiateNewMessageStep(
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
                "InitiateNewMessageStep.NoClient",
                "Unable to access signalR client");
        }
        
        Domain.Entity.Message? prevMsg = default;
        if (context.ConversationAppendData is not null)
        {
            prevMsg = context.Conversation!.Messages
                .FirstOrDefault(m => m.Id == context.ConversationAppendData.ExsistingMessageId);
        }

        var userMessage = new Domain.Entity.Message
        {
            Id = new MessageId(Guid.NewGuid()),
            PreviousMessage = prevMsg ?? context.Conversation!.Messages.FirstOrDefault(),
            Role = Domain.Entity.Role.User,
            Content = context.MessageContent,
            CreatedUtc = DateTime.UtcNow,
            CompletedUtc = DateTime.UtcNow,
            Visible = true,
        };
        context.Conversation!.Messages.Add(userMessage);
        context.Conversation.LastAppendedUtc = DateTime.UtcNow;
        await this.applicationContext.SaveChangesAsync();

        var rsvMsgDto = new ReceiveMessageDto
        {
            ConversationId = context.Conversation.Id.Value,
            Message = ConversationMapper.Map(userMessage),
        };
        await client.ReceiveMessage(rsvMsgDto);

        var assistantMessage = new Domain.Entity.Message
        {
            Id = new MessageId(Guid.NewGuid()),
            PreviousMessage = userMessage,
            Role = Domain.Entity.Role.Assistant,
            Content = string.Empty,
            CreatedUtc = DateTime.UtcNow,
            Visible = true,
        };
        context.AssistantMessage = assistantMessage;
        
        return context;
    }
}
