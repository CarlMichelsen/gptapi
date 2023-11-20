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
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IConversationService conversationService;

    public ConversationHandler(
        IHttpContextAccessor httpContextAccessor,
        IConversationService conversationService)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.conversationService = conversationService;
    }

    public async Task<Result<ConversationDto, string>> GetConversation(Guid conversationId)
    {
        var userId = this.GetUserId();
        var conversationResult = await this.conversationService
            .GetConversation(userId, conversationId);

        return conversationResult.Match<Result<ConversationDto, string>>(
            (conversation) => ConversationMapper.Map(conversation),
            (error) => error);
    }

    public Task<Result<List<ConversationMetaDataDto>, string>> GetConversations()
    {
        var userId = this.GetUserId();
        return this.conversationService.GetConversations(userId);
    }

    private string GetUserId()
    {
        var httpContext = this.httpContextAccessor.HttpContext;
        var claims = httpContext!.User.Claims.ToList();
        return claims.First(c => c.Type == "SteamId").Value;
    }
}
