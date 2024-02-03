using BusinessLogic.Map;
using Database;
using Domain;
using Domain.Abstractions;
using Domain.Dto.Conversation;
using Domain.Entity;
using Domain.Entity.Id;
using Domain.Exception;
using Interface.Factory;
using Interface.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Service;

public class ConversationService : IConversationService
{
    /*private readonly ILogger<ConversationService> logger;
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

    public async Task<DeprecatedResult<Conversation, string>> AppendConversation(
        UserProfileId userProfileId,
        ConversationId conversationId,
        Message message)
    {
        if (message.PreviousMessage is null)
        {
            throw new ServiceException("PreviousMessageId can't be null when attempting to append a conversation");
        }

        var conversationResult = await this.GetConversation(userProfileId, conversationId);
        var conv = conversationResult.Match<Conversation?>(
            c => c,
            _ => null);
        
        if (conv is null)
        {
            return "Could not find conversation to append.";
        }

        conv.Messages.Add(message);
        conv.LastAppendedUtc = DateTime.UtcNow;
        await this.applicationContext.SaveChangesAsync();
        this.logger.LogInformation(
                "Appended an exsisting conversation <{conversationId}>",
                conv.Id);

        return conv;
    }

    public async Task<DeprecatedResult<bool, string>> DeleteConversation(
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

    public async Task<DeprecatedResult<Conversation, string>> GetConversation(
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

    public async Task<DeprecatedResult<List<ConversationMetaDataDto>, string>> GetConversations(
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

    public async Task<DeprecatedResult<Conversation, string>> StartConversation(
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
    }*/

    private readonly ILogger<ConversationService> logger;
    private readonly ApplicationContext applicationContext;

    public ConversationService(
        ILogger<ConversationService> logger,
        ApplicationContext applicationContext)
    {
        this.logger = logger;
        this.applicationContext = applicationContext;
    }

    public Task<Result<Conversation>> AppendConversation(
        Guid userProfileId, 
        ConversationId conversationId,
        Message message)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteConversation(
        Guid userProfileId,
        ConversationId conversationId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Conversation>> GetConversation(
        Guid userProfileId,
        ConversationId conversationId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<List<ConversationMetaDataDto>>> GetConversationList(
        Guid userProfileId)
    {
        try
        {
            var convs = await this.applicationContext.Conversation
                .Where(c => c.UserProfileId == userProfileId && !c.UserArchived)
                .ToListAsync();

            return convs.Select(ConversationMapper.Map).ToList();
        }
        catch (Exception e)
        {
            this.logger.LogError(
                "ConversationService.GetConversationList threw an exception:\n{exception}",
                e);
            
            return new Error(
                "GetConversationList.Exception",
                "Failed to map domain model to DTO model");
        }
    }

    public Task<Result<bool>> SetConversationSummary(
        Guid userProfileId,
        ConversationId conversationId,
        string summary)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Conversation>> StartConversation(
        Guid userProfileId,
        Message message)
    {
        throw new NotImplementedException();
    }
}
