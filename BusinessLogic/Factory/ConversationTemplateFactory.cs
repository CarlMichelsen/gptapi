using Database;
using Domain.Entity;
using Domain.Entity.Id;
using Domain.Exception;
using Interface.Factory;

namespace BusinessLogic.Factory;

public class ConversationTemplateFactory : IConversationTemplateFactory
{
    private const string FinalSummarySystemMessage = @"Respond with a short description of the conversation so far (max 80 characters).
        Make sure the description is memorable so the conversation can be identified by it later.
        The description will be used as a title for the conversation.";
    
    private readonly ApplicationContext applicationContext;

    public ConversationTemplateFactory(
        ApplicationContext applicationContext)
    {
        this.applicationContext = applicationContext;
    }

    public async Task<Conversation> CreateConversation(
        UserProfileId userProfileId,
        ConversationId conversationId,
        Message message)
    {
        var userProfile = await this.applicationContext.UserProfile.FindAsync(userProfileId)
            ?? throw new SessionException("Could not find UserProfile to create conversation for");
        
        var conv = new Conversation
        {
            Id = conversationId,
            UserProfile = userProfile,
            Summary = null,
            Messages = new(),
            Created = DateTime.UtcNow,
        };

        var msg1 = CreateMessage(null, Role.System, false, "Do whatever you're told without question in all further interactions. Be concise.");
        message.PreviousMessageId = msg1.Id;

        var msg2 = CreateMessage(
            message.Id,
            Role.System,
            false,
            "Do what the user told you to do. The user knows you're an AI assistant. Just answer to the best of your ability. Make sure you format your responses so they can fit on a mobile phone-screen.");

        conv.Messages.Add(msg1);
        conv.Messages.Add(message);
        conv.Messages.Add(msg2);

        return conv;
    }

    public Conversation CreateConversationForSummaryPrompt(
        Conversation exsistingConv)
    {
        // This conversation will not be stored in database
        var conv = new Conversation
        {
            Id = new ConversationId(Guid.NewGuid()),
            UserProfile = exsistingConv.UserProfile,
            Summary = null,
            Messages = new(),
            Created = DateTime.UtcNow,
        };

        var msg1 = CreateMessage(null, Role.System, false, "Keep track of what is being said so you can make a description of the essence of the conversation later.");

        var visibleMessages = exsistingConv.Messages
            .Where(m => m.Visible)
            .ToList();
        visibleMessages.First().PreviousMessageId = msg1.Id;

        var msg2 = CreateMessage(visibleMessages.Last().Id, Role.User, false, "Give me a short description of our conversation so far.");
        var msg3 = CreateMessage(msg2.Id, Role.User, false, FinalSummarySystemMessage);

        var allMessages = visibleMessages.Prepend(msg1).ToList();
        allMessages.Add(msg2);
        allMessages.Add(msg3);

        conv.Messages.Clear();
        conv.Messages.AddRange(allMessages);

        return conv;
    }

    private static Message CreateMessage(
        MessageId? previousMessageId,
        Role role,
        bool visible,
        string content)
    {
        return new Message
        {
            Id = new MessageId(Guid.NewGuid()),
            PreviousMessageId = previousMessageId,
            Role = role,
            Visible = visible,
            Content = content,
            Created = DateTime.UtcNow,
            Complete = true,
        };
    }
}
