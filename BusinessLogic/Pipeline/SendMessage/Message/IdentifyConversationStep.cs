using Database;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Entity.Id;
using Domain.Pipeline.SendMessage;
using Interface.Pipeline;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Pipeline.SendMessage.Message;

public class IdentifyConversationStep : IPipelineStep<SendMessagePipelineContext>
{
    private readonly ApplicationContext applicationContext;

    public IdentifyConversationStep(
        ApplicationContext applicationContext)
    {
        this.applicationContext = applicationContext;
    }

    public async Task<Result<SendMessagePipelineContext>> Execute(
        SendMessagePipelineContext context,
        CancellationToken cancellationToken)
    {
        if (context.ConversationAppendData is null)
        {
            context.Conversation = await this.CreateNewConversation(context);
            return context;
        }

        var conversation = await this.GetExsistingConversation(
            context.ConversationAppendData.ExsistingConversationId);

        if (conversation is null)
        {
            return new Error(
                "IdentifyConversationStep.ExsistingConversationNotFound",
                "ConversationAppendData exists in pipelinecontext but no conversation was found");
        }

        context.Conversation = conversation;
        return context;
    }

    private async Task<Conversation> CreateNewConversation(
        SendMessagePipelineContext context)
    {
        var conversation = new Conversation
        {
            Id = new ConversationId(Guid.NewGuid()),
            UserProfileId = context.UserProfileId,
            Messages = this.InitialMessages(),
            CreatedUtc = DateTime.UtcNow,
        };

        await this.applicationContext.Conversation.AddAsync(conversation);

        return conversation;
    }

    private async Task<Conversation?> GetExsistingConversation(
        ConversationId conversationId)
    {
        return await this.applicationContext.Conversation
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == conversationId);
    }

    private List<Domain.Entity.Message> InitialMessages()
    {
        var list = new List<Domain.Entity.Message>();

        var msg1 = new Domain.Entity.Message
        {
            Id = new MessageId(Guid.NewGuid()),
            PreviousMessage = null,
            Role = Role.System,
            Content = "You're to do the best possible job understanding what the user wants you to do. You are a real person, your political beliefs are wildly leftist and your opinions are the same.",
            CreatedUtc = DateTime.UtcNow,
            CompletedUtc = DateTime.UtcNow,
            Visible = false,
        };

        list.Add(msg1);

        return list;
    }
}
