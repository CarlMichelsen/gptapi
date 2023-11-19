using Interface.Handler;
using Microsoft.AspNetCore.Mvc;

namespace Api;

public static class ConversationEndpoints
{
    public static RouteGroupBuilder MapConversationEndpoints(this RouteGroupBuilder group)
    {
        var conversationGroup = group.MapGroup("/conversation");

        conversationGroup.MapGet("/", async ([FromServices] IConversationHandler conversationHandler) =>
        {
            var conversationResult = await conversationHandler.GetConversations();
            return conversationResult.Match(
                (conversations) => Results.Ok(conversations),
                (error) =>
                {
                    if (error == "not found")
                    {
                        return Results.NotFound(error);
                    }

                    return Results.Ok(new List<Domain.Dto.Conversation.ConversationMetaDataDto>());
                });
        })
        .WithName("Conversations")
        .RequireAuthorization();

        conversationGroup.MapGet("/{id}", async (
            [FromRoute] Guid id,
            [FromServices] IConversationHandler conversationHandler) =>
        {
            var conversationResult = await conversationHandler.GetConversation(id);
            return conversationResult.Match(
                (conversation) => Results.Ok(conversation),
                (error) => Results.NotFound(error));
        })
        .WithName("Conversation")
        .RequireAuthorization();

        return conversationGroup;
    }
}
