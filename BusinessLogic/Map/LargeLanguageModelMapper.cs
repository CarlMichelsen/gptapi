using Domain.Entity;
using Domain.Exception;
using Domain.LargeLanguageModel.Shared.Request;

namespace BusinessLogic.Map;

public static class LargeLanguageModelMapper
{
    public static LlmRequest Map(
        Conversation conversation,
        string modelString,
        string systemMessage,
        int maxTokens)
    {
        return new LlmRequest
        {
            SystemMessage = systemMessage,
            ModelVersion = new LlmModelVersion { Model = modelString },
            Messages = conversation.Messages.Select(Map).ToList(),
            MaxTokens = maxTokens,
        };
    }

    public static LlmMessage Map(Message message)
    {
        return new LlmMessage
        {
            Role = Map(message.Role),
            Content = message.Content ?? string.Empty,
        };
    }

    public static LlmRole Map(Role role)
    {
        return role switch {
            Role.User => LlmRole.User,
            Role.Assistant => LlmRole.Assistant,
            Role.System => throw new LargeLanguageModelException("Cannot convert Role.System to LlmRole"),
            _ => throw new LargeLanguageModelException("Failed to convert conversation role to LargeLanguageModelMessageRole"),
        };
    }
}
