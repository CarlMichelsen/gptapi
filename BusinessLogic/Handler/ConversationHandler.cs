using BusinessLogic.Map;
using Domain;
using Domain.Claims;
using Domain.Dto.Conversation;
using Domain.Entity.Id;
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

    public async Task<Result<bool, string>> DeleteConversation(ConversationId conversationId)
    {
        var userProfileId = this.GetUserProfileId();
        var conversationResult = await this.conversationService
            .DeleteConversation(userProfileId, conversationId);
        
        return conversationResult.Match<Result<bool, string>>(
            (conversation) => true,
            (error) => false);
    }

    public async Task<Result<ConversationDto, string>> GetConversation(ConversationId conversationId)
    {
        var userProfileId = this.GetUserProfileId();
        var conversationResult = await this.conversationService
            .GetConversation(userProfileId, conversationId);

        return conversationResult.Match<Result<ConversationDto, string>>(
            (conversation) => ConversationMapper.Map(conversation),
            (error) => error);
    }

    public Task<Result<List<ConversationMetaDataDto>, string>> GetConversations()
    {
        var userProfileId = this.GetUserProfileId();
        return this.conversationService.GetConversations(userProfileId);
    }

    private string GetClaimValue(string claimType)
    {
        var httpContext = this.httpContextAccessor.HttpContext;
        var claims = httpContext!.User.Claims.ToList();
        return claims.First(c => c.Type == claimType).Value;
    }

    private UserProfileId GetUserProfileId()
    {
        var stringId = this.GetClaimValue(GptClaimKeys.UserProfileId);
        var guid = Guid.Parse(stringId);
        return new UserProfileId(guid);
    }
}
