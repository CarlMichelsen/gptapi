using Domain.Entity;
using Domain.Exception;
using Domain.LargeLanguageModel.Shared;

namespace BusinessLogic.Map;

public static class LargeLanguageModelMapper
{
    public static LargeLanguageModelRequest Map(Conversation conversation, string modelString)
    {
        return new LargeLanguageModelRequest
        {
            ModelVersion = new LargeLanguageModelVersion { Model = modelString },
            Messages = conversation.Messages.Select(Map).ToList(),
        };
    }

    public static LargeLanguageModelMessage Map(Message message)
    {
        return new LargeLanguageModelMessage
        {
            Role = Map(message.Role),
            Content = message.Content ?? string.Empty,
        };
    }

    public static LargeLanguageModelMessageRole Map(Role role)
    {
        return role switch {
            Role.User => LargeLanguageModelMessageRole.User,
            Role.Assistant => LargeLanguageModelMessageRole.Assistant,
            Role.System => LargeLanguageModelMessageRole.System,
            _ => throw new LargeLanguageModelException("Failed to convert conversation role to LargeLanguageModelMessageRole"),
        };
    }
}
