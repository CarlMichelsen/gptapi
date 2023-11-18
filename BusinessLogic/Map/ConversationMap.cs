using Domain.Dto.Conversation;
using Domain.Entity;

namespace BusinessLogic.Map;

public static class ConversationMap
{
    public static ConversationDto Map(Conversation conversation)
    {
        return new ConversationDto
        {
            Id = conversation.Id,
            Messages = conversation.Messages.Select(Map).ToList(),
        };
    }

    public static MessageDto Map(Message message)
    {
        return new MessageDto
        {
            Id = message.Id,
            Role = Map(message.Role),
            Content = message.Content,
        };
    }

    public static string Map(Role role)
    {
        return Enum.GetName(role)?.ToLower() ?? throw new MapException("Message role enum failed to map properly");
    }
}
