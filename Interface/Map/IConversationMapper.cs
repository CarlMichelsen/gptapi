using Domain.Abstractions;
using Domain.Dto.Conversation;
using Domain.Entity;

namespace Interface.Map;

public interface IConversationMapper
{
    Task<Result<ConversationDto>> MapConversation(Conversation conversation);
}
