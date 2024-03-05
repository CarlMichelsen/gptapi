using Domain.Entity;
using Domain.Gpt;

namespace BusinessLogic.Map;

public static class GptMapper
{
    public static GptChatPrompt Map(Conversation conversation)
    {
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