using Domain.Exception;
using Domain.LargeLanguageModel.Claude;
using Domain.LargeLanguageModel.Shared;

namespace BusinessLogic.Map.LargeLanguageModel;

public static class ClaudeMapper
{
    public static ClaudePrompt Map(LargeLanguageModelRequest request, int maxTokens)
    {
        return new ClaudePrompt
        {
            Model = request.ModelVersion.Model,
            MaxTokens = maxTokens,
            Messages = request.Messages.Select(Map).ToList(),
            Stream = request.Stream,
        };
    }

    public static ClaudeMessage Map(LargeLanguageModelMessage message)
    {
        return new ClaudeMessage
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
                return "assistant";

            default:
                throw new LargeLanguageModelException("Failed to map role for GptChatPrompt");
        }
    }
}
