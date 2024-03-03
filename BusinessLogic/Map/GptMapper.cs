using Domain.Entity;
using Domain.Exception;
using Domain.Gpt;

namespace BusinessLogic.Map;

public static class GptMapper
{
    public static GptChatPrompt Map(Conversation conversation)
    {
        var lastMessage = conversation.Messages.Last();
        if (lastMessage.Role == Role.Assistant)
        {
            throw new MapException("Last message in conversation can't be from the assistant in order to map to a GptChatPrompt");
        }

        return new GptChatPrompt
        {
            Model = "gpt-4",
            Messages = conversation.Messages.Select(Map).ToList(),
        };
    }

    public static GptChatMessage Map(Message message)
    {
        return new GptChatMessage
        {
            Role = ConversationMapper.Map(message.Role),
            Content = message.Content ?? string.Empty,
        };
    }
}