using Microsoft.AspNetCore.Http;

namespace Interface.Handler;

public interface IConversationHandler
{
    Task<IResult> GetConversationList();

    Task<IResult> GetConversation(Guid conversationId);

    Task<IResult> DeleteConversation(Guid conversationId);
}
