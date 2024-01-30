using BusinessLogic.Hub;
using Database;
using Domain.Abstractions;
using Domain.Dto.Conversation;
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
            Id = new Domain.Entity.Id.MessageId(Guid.NewGuid()),
            PreviousMessage = prevMsg,
            Role = Domain.Entity.Role.User,
            Content = context.UserMessageData.MessageContent,
            CreatedUtc = DateTime.UtcNow,
            CompletedUtc = DateTime.UtcNow,
        };
        context.Conversation!.Messages.Add(userMessage);
        await this.applicationContext.SaveChangesAsync();

        var assistantMessage = new Domain.Entity.Message
        {
            Id = new Domain.Entity.Id.MessageId(Guid.NewGuid()),
            PreviousMessage = userMessage,
            Role = Domain.Entity.Role.Assistant,
            Content = default,
            CreatedUtc = DateTime.UtcNow,
        };
        context.AssistantMessage = assistantMessage;
        
        var updateMessageId = new UpdateMessageIdDto(
            context.UserMessageData.TemporaryUserMessageId,
            userMessage.Id.Value,
            assistantMessage.Id.Value,
            context.Conversation!.Id.Value);
        await client.UpdateMessageId(updateMessageId);
        
        return context;
    }
}
