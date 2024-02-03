using Domain.Dto;
using Domain.Dto.Conversation;
using Interface.Handler;
using Interface.Service;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Handler;

public class ConversationHandler : BaseHandler, IConversationHandler
{
    private readonly IConversationService conversationService;

    public ConversationHandler(
        IConversationService conversationService,
        ISessionService sessionService)
        : base(sessionService)
    {
        this.conversationService = conversationService;
    }

    public Task<IResult> DeleteConversation(Guid conversationId)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> GetConversation(Guid conversationId)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> GetConversationList()
    {
        var sessionData = await this.GetSession();
        var conversationListResult = await this.conversationService
            .GetConversationList(sessionData.UserProfileId);
        
        return conversationListResult.Match(
            success => Results.Ok(new ServiceResponse<List<ConversationMetaDataDto>>(success)),
            failure => Results.StatusCode(500));
    }
}
