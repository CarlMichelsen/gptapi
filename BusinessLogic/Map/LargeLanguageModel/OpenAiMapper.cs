using Domain.Exception;
using Domain.LargeLanguageModel.OpenAi;
using Domain.LargeLanguageModel.Shared;

namespace BusinessLogic.Map.LargeLanguageModel;

public static class OpenAiMapper
{
    public static GptChatPrompt Map(LargeLanguageModelRequest request)
    {
        return new GptChatPrompt
        {
            Model = request.ModelVersion.Model,
            Messages = request.Messages.Select(Map).ToList(),
            Stream = request.Stream,
        };
    }

    public static GptChatMessage Map(LargeLanguageModelMessage message)
    {
        return new GptChatMessage
        {
            Role = Map(message.Role),
            Content = message.Content,
        };
    }

    public static string Map(LargeLanguageModelMessageRole role)
    {
        switch (role)
        {
            case LargeLanguageModelMessageRole.Assistant:
                return "assistant";
            
            case LargeLanguageModelMessageRole.User:
                return "user";

            case LargeLanguageModelMessageRole.System:
                return "system";

            default:
                throw new LargeLanguageModelException("Failed to map role for GptChatPrompt");
        }
    }
}
