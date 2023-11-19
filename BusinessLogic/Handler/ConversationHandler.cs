using BusinessLogic.Map;
using Database;
using Domain;
using Domain.Dto.Conversation;
using Interface.Handler;
using Interface.Service;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Handler;

public class ConversationHandler : IConversationHandler
{
    private readonly ApplicationContext applicationContext;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IConversationService conversationService;

    public ConversationHandler(
        ApplicationContext applicationContext,
        IHttpContextAccessor httpContextAccessor,
        IConversationService conversationService)
    {
        this.applicationContext = applicationContext;
        this.httpContextAccessor = httpContextAccessor;
        this.conversationService = conversationService;
    }

    public async Task<Result<ConversationDto, string>> GetConversation(Guid conversationId)
    {
        var userId = this.GetUserId();
        var conversationResult = await this.conversationService
            .GetConversation(this.applicationContext, userId, conversationId);

        return conversationResult.Match<Result<ConversationDto, string>>(
            (conversation) => ConversationMapper.Map(conversation),
            (error) => error);
    }

    public Task<Result<List<ConversationMetaDataDto>, string>> GetConversations()
    {
        var userId = this.GetUserId();
        return this.conversationService.GetConversations(this.applicationContext, userId);
    }

    private string GetUserId()
    {
        var httpContext = this.httpContextAccessor.HttpContext;
        var claims = httpContext!.User.Claims.ToList();
        return claims.First(c => c.Type == "SteamId").Value;
    }
}
