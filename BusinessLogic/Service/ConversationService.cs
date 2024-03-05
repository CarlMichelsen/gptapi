using BusinessLogic.Map;
using Database;
using Domain.Abstractions;
using Domain.Dto.Conversation;
using Domain.Entity;
using Domain.Entity.Id;
using Interface.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Service;

public class ConversationService : IConversationService
{
    private readonly ILogger<ConversationService> logger;
    private readonly ApplicationContext applicationContext;

    public ConversationService(
        ILogger<ConversationService> logger,
        ApplicationContext applicationContext)
    {
        this.logger = logger;
        this.applicationContext = applicationContext;
    }

    public async Task<Result<bool>> DeleteConversation(
        Guid userProfileId,
        ConversationId conversationId)
    {
        try
        {
            var affectedRows = await this.applicationContext.Conversation
                .Where(c => c.UserProfileId == userProfileId)
                .Where(c => c.Id == conversationId)
                .Where(c => !c.UserArchived)
                .ExecuteUpdateAsync(c => c.SetProperty(b => b.UserArchived, true));
            
            if (affectedRows == 0)
            {
                return false;
            }
            
            if (affectedRows > 1)
            {
                return new Error(
                    "ConversationService.ManyDeleted",
                    $"{affectedRows} rows were affected by the deletion.");
            }

            return true;
        }
        catch (Exception e)
        {
            this.logger.LogError(
                "ConversationService.DeleteConversation threw an exception:\n{exception}",
                e);

            return new Error(
                "ConversationService.Exception",
                "An error occured when fetching the conversation");
        }
    }

    public async Task<Result<Conversation>> GetConversation(
        Guid userProfileId,
        ConversationId conversationId)
    {
        try
        {
            var conv = await this.applicationContext.Conversation
                .Where(c => c.UserProfileId == userProfileId)
                .Where(c => c.Id == conversationId)
                .Where(c => !c.UserArchived)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync();
            
            if (conv is null)
            {
                return new Error(
                    "ConversationService.NotFound",
                    "No conversation owned by the userProfileId has the given conversationId");
            }

            this.applicationContext.Entry(conv).State = EntityState.Detached;
            return conv;
        }
        catch (Exception e)
        {
            this.logger.LogError(
                "ConversationService.GetConversation threw an exception:\n{exception}",
                e);

            return new Error(
                "ConversationService.Exception",
                "An error occured when fetching the conversation");
        }
    }

    public async Task<Result<List<ConversationDateChunkDto>>> GetConversationList(
        Guid userProfileId)
    {
        try
        {
            var convs = await this.applicationContext.Conversation
                .Where(c => c.UserProfileId == userProfileId && !c.UserArchived)
                .ToListAsync();
            
            var conversationOptions = convs
                .Select(ConversationMapper.MapToMetaDataDto)
                .ToList();

            return ConversationMapper.Map(conversationOptions);
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

    public async Task<Result<bool>> SetConversationSummary(
        Guid userProfileId,
        ConversationId conversationId,
        string summary)
    {
        try
        {
            var affectedRows = await this.applicationContext.Conversation
                .Where(c => c.UserProfileId == userProfileId)
                .Where(c => c.Id == conversationId)
                .Where(c => !c.UserArchived)
                .ExecuteUpdateAsync(c => c.SetProperty(b => b.Summary, summary));

            if (affectedRows == 0)
            {
                return false;
            }
            
            if (affectedRows > 1)
            {
                return new Error(
                    "ConversationService.ManySummariesAdded",
                    $"{affectedRows} rows were updated when adding single summary.");
            }

            return true;
        }
        catch (Exception e)
        {
            this.logger.LogError(
                "ConversationService.SetConversationSummary threw an exception:\n{exception}",
                e);

            return new Error(
                "ConversationService.Exception",
                "An error occured when adding summary to the conversation");
        }
    }
}
