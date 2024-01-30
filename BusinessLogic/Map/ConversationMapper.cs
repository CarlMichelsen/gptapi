using Domain.Dto.Conversation;
using Domain.Entity;
using Domain.Exception;

namespace BusinessLogic.Map;

public static class ConversationMapper
{
    public static MessageDto Map(Message message)
    {
        return new MessageDto
        {
            Id = message.Id.Value,
            Role = Map(message.Role),
            Content = message.Content ?? throw new MapException("Message content null when mapping message"),
            Complete = message.CompletedUtc is not null,
            Created = message.CreatedUtc,
        };
    }

    public static string Map(Role role)
    {
        return Enum.GetName(role)?.ToLower() ?? throw new MapException("Message role enum failed to map properly");
    }
}
