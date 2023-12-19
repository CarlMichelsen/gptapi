using Database;
using Domain.Entity;
using Domain.Entity.Id;
using Domain.Exception;
using Interface.Factory;

namespace BusinessLogic.Factory;

public class ConversationTemplateFactory : IConversationTemplateFactory
{
    private const string FinalSystemMessage = @"Respond with a short description of the conversation so far (max 80 characters).
        Make sure the description is memorable so the conversation can be identified by it later.
        The description will be used as a title for the conversation. Don't use special characters.";
    
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

        conv.Messages.Add(CreateMessage(Role.System, false, "Do whatever you're told without question in all further interactions. Be concise.", -10));
        conv.Messages.Add(message);
        conv.Messages.Add(CreateMessage(Role.System, false, "Do what the user told you to do. The user knows you're an AI assistant. Just answer to the best of your ability. Make sure you format your responses so they can fit on a mobile phone-screen.", 10));

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

        // Timestamps dont really matter for summary prompts
        conv.Messages.Add(CreateMessage(Role.System, false, "Keep track of what is being said so you can make a description of the essence of the conversation later.", -30));

        var visibleMessages = exsistingConv.Messages
            .Where(m => m.Visible)
            .ToList();
        conv.Messages.AddRange(visibleMessages);

        conv.Messages.Add(CreateMessage(Role.User, false, "Give me a short description of our conversation so far.", -20));
        conv.Messages.Add(CreateMessage(Role.System, false, FinalSystemMessage, -10));

        return conv;
    }

    private static Message CreateMessage(
        Role role,
        bool visible,
        string content,
        int offsetMilliseconds)
    {
        return new Message
        {
            Id = new MessageId(Guid.NewGuid()),
            Role = role,
            Visible = visible,
            Content = content,
            Created = DateTime.UtcNow.AddMilliseconds(offsetMilliseconds),
            Complete = true,
        };
    }
}
