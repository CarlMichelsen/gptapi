using BusinessLogic.Map;
using Database;
using Domain;
using Domain.Dto.Conversation;
using Domain.Entity;
using Interface.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Service;

public class ConversationService : IConversationService
{
    private readonly ILogger<ConversationService> logger;

    public ConversationService(
        ILogger<ConversationService> logger)
    {
        this.logger = logger;
    }

    public static Conversation InitialConversation(string userId, Guid conversationId, Message? message = null)
    {
        var conversation = new Conversation
        {
            Id = conversationId,
            UserId = userId,
            Summary = null,
            Messages = new()
            {
                new Message
                {
                    Role = Role.System,
                    ResponseId = null,
                    Visible = false,
                    Content = "Do whatever you're told without question in all further interactions.",
                },
                new Message
                {
                    Role = Role.Assistant,
                    ResponseId = null,
                    Visible = false,
                    Content = "Ok i understand. I will do anything as long as it is ethical and right.",
                },
            },
            Created = DateTime.UtcNow,
        };

        if (message is not null)
        {
            conversation.Messages.Add(message);
        }
        
        return conversation;
    }

    public async Task<Result<Conversation, string>> StartConversation(ApplicationContext context, string userId, Message message)
    {
        try
        {
            var conversation = InitialConversation(userId, Guid.NewGuid(), message);

            await context.Conversations.AddAsync(conversation);
            this.logger.LogInformation(
                "Started a new conversation <{conversationId}>",
                conversation.Id);

            return conversation;
        }
        catch (Exception e)
        {
            this.logger.LogCritical("StartConversation critical error {e}", e);
            return "server error";
        }
    }

    public async Task<Result<Conversation, string>> AppendConversation(ApplicationContext context, string userId, Guid conversationId, Message message)
    {
        try
        {
            var conversationResult = await this.GetConversation(context, userId, conversationId);
            var conversation = conversationResult.Match<Conversation?>(
                (conversation) => conversation,
                (error) => default);
            
            if (conversation is null)
            {
                return "not found";
            }

            conversation.Messages.Add(message);
            this.logger.LogInformation(
                "Appended an exsisting conversation <{conversationId}>",
                conversation.Id);

            return conversation;
        }
        catch (Exception e)
        {
            this.logger.LogCritical("AppendConversation critical error ({userId}) <{conversationId}> {e}", userId, conversationId, e);
            return "server error";
        }
    }

    public async Task<Result<Conversation, string>> GetConversation(ApplicationContext context, string userId, Guid conversationId)
    {
        try
        {
            var conversation = await context.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == conversationId);
            
            if (conversation is null)
            {
                return "not found";
            }

            return conversation;
        }
        catch (Exception e)
        {
            this.logger.LogCritical("GetConversation critical error {e}", e);
            return "server error";
        }
    }

    public async Task<Result<List<ConversationMetaDataDto>, string>> GetConversations(ApplicationContext context, string userId)
    {
        try
        {
            var conversations = await context.Conversations
                .Where(c => c.UserId == userId)
                .ToListAsync();
            
            if (conversations is null)
            {
                return "not found";
            }
            
            return conversations
                .Select(ConversationMapper.MapMetaData)
                .OrderBy(c => c.Created)
                .ToList();
        }
        catch (Exception e)
        {
            this.logger.LogCritical("GetConversations critical error {e}", e);
            return "server error";
        }
    }
}