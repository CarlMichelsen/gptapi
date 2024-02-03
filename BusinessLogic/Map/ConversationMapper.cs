using Domain.Dto.Conversation;
using Domain.Entity;

namespace BusinessLogic.Map;

public static class ConversationMapper
{
    public static ConversationMetaDataDto Map(Conversation conversation)
    {
        return new ConversationMetaDataDto(
            conversation.Id.Value,
            conversation.Summary ?? "New conversation",
            conversation.LastAppendedUtc,
            conversation.CreatedUtc);
    }
}
