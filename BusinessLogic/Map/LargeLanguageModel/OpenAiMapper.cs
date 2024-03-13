using Domain.Exception;
using Domain.LargeLanguageModel.OpenAi;
using Domain.LargeLanguageModel.Shared.Request;

namespace BusinessLogic.Map.LargeLanguageModel;

public static class OpenAiMapper
{
    public static GptChatPrompt Map(LlmRequest request)
    {
        return new GptChatPrompt
        {
            Model = request.ModelVersion.Model,
            Messages = request.Messages.Select(Map).ToList(),
            Stream = request.Stream,
        };
    }

    public static GptChatMessage Map(LlmMessage message)
    {
        return new GptChatMessage
        {
            Role = Map(message.Role),
            Content = message.Content,
        };
    }

    public static string Map(LlmRole role)
    {
        switch (role)
        {
            case LlmRole.Assistant:
                return "assistant";
            
            case LlmRole.User:
                return "user";

            default:
                throw new LargeLanguageModelException("Failed to map role for GptChatPrompt");
        }
    }
}
