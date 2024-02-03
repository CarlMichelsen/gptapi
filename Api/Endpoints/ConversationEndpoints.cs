using Domain.Entity.Id;
using Interface.Handler;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class ConversationEndpoints
{
    public static RouteGroupBuilder MapConversationEndpoints(this RouteGroupBuilder group)
    {
        var conversationGroup = group.MapGroup("/conversation");

        conversationGroup.MapGet("/", async ([FromServices] IConversationHandler conversationHandler) =>
            await conversationHandler.GetConversationList())
        .WithName("Conversations")
        .RequireAuthorization();

        conversationGroup.MapGet("/{conversationId}", async (
            [FromRoute] Guid conversationId,
            [FromServices] IConversationHandler conversationHandler) =>
            await conversationHandler.GetConversation(conversationId))
        .WithName("Conversation")
        .RequireAuthorization();

        conversationGroup.MapDelete("/{conversationId}", async (
            [FromRoute] Guid conversationId,
            [FromServices] IConversationHandler conversationHandler) =>
            await conversationHandler.DeleteConversation(conversationId))
        .WithName("DeleteConversation")
        .RequireAuthorization();

        return group;
    }
}
