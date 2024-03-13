using Domain.Exception;
using Domain.LargeLanguageModel.Claude;
using Domain.LargeLanguageModel.Shared.Request;

namespace BusinessLogic.Map.LargeLanguageModel;

public static class ClaudeMapper
{
    public static ClaudePrompt Map(LlmRequest request, int maxTokens)
    {
        return new ClaudePrompt
        {
            Model = request.ModelVersion.Model,
            MaxTokens = maxTokens,
            Messages = request.Messages.Select(Map).ToList(),
            Stream = request.Stream,
        };
    }

    public static ClaudeMessage Map(LlmMessage message)
    {
        return new ClaudeMessage
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
