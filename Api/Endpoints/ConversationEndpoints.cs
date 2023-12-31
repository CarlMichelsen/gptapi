﻿using Domain.Entity.Id;
using Interface.Handler;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

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
            var conversationResult = await conversationHandler.GetConversation(new ConversationId(id));
            return conversationResult.Match(
                (conversation) => Results.Ok(conversation),
                (error) => Results.NotFound(error));
        })
        .WithName("Conversation")
        .RequireAuthorization();

        conversationGroup.MapDelete("/{id}", async (
            [FromRoute] Guid id,
            [FromServices] IConversationHandler conversationHandler) =>
        {
            var conversationDeletedResult = await conversationHandler.DeleteConversation(new ConversationId(id));
            return conversationDeletedResult.Match(
                (conversationDeleted) => Results.Ok(conversationDeleted),
                (error) => Results.NotFound(error));
        })
        .WithName("DeleteConversation")
        .RequireAuthorization();

        return group;
    }
}
