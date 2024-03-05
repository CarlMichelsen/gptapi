using Domain.Dto;
using Domain.Dto.Conversation;
using Domain.Entity.Id;
using Interface.Handler;
using Interface.Map;
using Interface.Service;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Handler;

public class ConversationHandler : BaseHandler, IConversationHandler
{
    private readonly IConversationMapper conversationMapper;
    private readonly IConversationService conversationService;

    public ConversationHandler(
        IConversationMapper conversationMapper,
        IConversationService conversationService,
        ISessionService sessionService)
        : base(sessionService)
    {
        this.conversationMapper = conversationMapper;
        this.conversationService = conversationService;
    }

    public async Task<IResult> DeleteConversation(
        Guid conversationId)
    {
        var sessionData = await this.GetSession();
        var deleteResult = await this.conversationService.DeleteConversation(
            sessionData.UserProfileId,
            new ConversationId(conversationId));
        
        return deleteResult.Match(
            success => Results.Ok(new ServiceResponse<bool>(success)),
            failure => Results.Ok(new ServiceResponse<bool>(failure.Code)));
    }

    public async Task<IResult> GetConversation(
        Guid conversationId)
    {
        var sessionData = await this.GetSession();
        var conversationResult = await this.conversationService.GetConversation(
            sessionData.UserProfileId,
            new ConversationId(conversationId));
        
        if (conversationResult.IsError)
        {
            var res = new ServiceResponse<ConversationDto>(conversationResult.Error!.Code);
            return Results.Ok(res);
        }
        
        var mappedConversationResult = await this.conversationMapper
            .MapConversation(conversationResult.Unwrap());
        
        return mappedConversationResult.Match(
            success => Results.Ok(new ServiceResponse<ConversationDto>(success)),
            failure => Results.Ok(new ServiceResponse<ConversationDto>(failure.Code)));
    }

    public async Task<IResult> GetConversationList()
    {
        var sessionData = await this.GetSession();
        var conversationListResult = await this.conversationService
            .GetConversationList(sessionData.UserProfileId);
        
        return conversationListResult.Match(
            success => Results.Ok(new ServiceResponse<List<ConversationDateChunkDto>>(success)),
            failure => Results.Ok(new ServiceResponse<List<ConversationDateChunkDto>>(failure.Code)));
    }
}
