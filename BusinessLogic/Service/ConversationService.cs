using BusinessLogic.Map;
using Database;
using Domain;
using Domain.Dto.Conversation;
using Domain.Entity;
using Domain.Entity.Id;
using Interface.Factory;
using Interface.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Service;

public class ConversationService : IConversationService
{
    private readonly ILogger<ConversationService> logger;
    private readonly ApplicationContext applicationContext;
    private readonly IConversationTemplateFactory conversationTemplateFactory;

    public ConversationService(
        ILogger<ConversationService> logger,
        ApplicationContext applicationContext,
        IConversationTemplateFactory conversationTemplateFactory)
    {
        this.logger = logger;
        this.applicationContext = applicationContext;
        this.conversationTemplateFactory = conversationTemplateFactory;
    }

    public async Task<Result<Conversation, string>> AppendConversation(
        UserProfileId userProfileId,
        ConversationId conversationId,
        Message message)
    {
        var conversationResult = await this.GetConversation(userProfileId, conversationId);
        var conv = conversationResult.Match<Conversation?>(
            c => c,
            _ => null);
        
        if (conv is null)
        {
            return "Could not find conversation to append.";
        }

        conv.Messages.Add(message);
        conv.LastAppended = DateTime.UtcNow;
        await this.applicationContext.SaveChangesAsync();
        this.logger.LogInformation(
                "Appended an exsisting conversation <{conversationId}>",
                conv.Id);

        return conv;
    }

    public async Task<Result<bool, string>> DeleteConversation(
        UserProfileId userProfileId,
        ConversationId conversationId)
    {
        var conv = await this.applicationContext.Conversation
            .Include(c => c.UserProfile)
            .FirstOrDefaultAsync(c => c.UserProfile.Id == userProfileId && c.Id == conversationId && !c.UserDeleted);
        
        if (conv is null)
        {
            return "Could not find conversation to delete";
        }

        conv.UserDeleted = true;
        await this.applicationContext.SaveChangesAsync();

        return true;
    }

    public async Task<Result<Conversation, string>> GetConversation(
        UserProfileId userProfileId,
        ConversationId conversationId)
    {
        var conv = await this.applicationContext.Conversation
            .Include(c => c.UserProfile)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.UserProfile.Id == userProfileId && c.Id == conversationId && !c.UserDeleted);
        
        if (conv is null)
        {
            return "Could not find conversation";
        }

        return conv;
    }

    public async Task<Result<List<ConversationMetaDataDto>, string>> GetConversations(
        UserProfileId userProfileId)
    {
        var convs = await this.applicationContext.Conversation
            .Include(c => c.UserProfile)
            .Where(c => c.UserProfile.Id == userProfileId && !c.UserDeleted)
            .ToListAsync();
        
        if (convs is null)
        {
            return "not found";
        }

        return convs
            .OrderByDescending(c => c.LastAppended)
            .Select(ConversationMapper.MapMetaData)
            .ToList();
    }

    public async Task<bool> SetConversationSummary(
        UserProfileId userProfileId,
        ConversationId conversationId,
        string summary)
    {
        var conversation = await this.applicationContext.Conversation
            .Include(c => c.UserProfile)
            .FirstOrDefaultAsync(c => c.UserProfile.Id == userProfileId && c.Id == conversationId && !c.UserDeleted);

        if (conversation is null)
        {
            return false;
        }

        conversation.Summary = summary;
        await this.applicationContext.SaveChangesAsync();

        return true;
    }

    public async Task<Result<Conversation, string>> StartConversation(
        UserProfileId userProfileId,
        Message message)
    {
        var conv = await this.conversationTemplateFactory.CreateConversation(
            userProfileId,
            new ConversationId(Guid.NewGuid()),
            message);
        
        await this.applicationContext.Conversation.AddAsync(conv);
        await this.applicationContext.SaveChangesAsync();

        this.logger.LogInformation(
            "Started a new conversation <{conversationId}>",
            conv.Id);
        
        return conv;
    }
}
