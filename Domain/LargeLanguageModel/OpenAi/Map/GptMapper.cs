using Domain.Exception;
using Domain.LargeLanguageModel.Shared.Request;
using Domain.LargeLanguageModel.Shared.Response;
using Domain.LargeLanguageModel.Shared.Stream;

namespace Domain.LargeLanguageModel.OpenAi.Map;

public static class GptMapper
{
    public static LlmResponse Map(GptChatResponse response)
    {
        return new LlmResponse
        {
            Id = response.Id,
            Model = MapModel(response.Model),
            Choices = response.Choices.Select(Map).ToList(),
            Usage = Map(response.Usage),
        };
    }

    public static LlmChunk Map(GptChatStreamChunk gptChatStreamChunk)
    {
        return new LlmChunk
        {
            Choices = gptChatStreamChunk.Choices.Select(Map).ToList(),
        };
    }

    /// <summary>
    /// Maps GptStreamChoice to LlmContent assuming role is assistant.
    /// </summary>
    /// <param name="gptStreamChoice">GptStreamChoice assumed to be assistant role.</param>
    /// <returns>LlmContent.</returns>
    public static LlmContent Map(GptStreamChoice gptStreamChoice)
    {
        return new LlmContent
        {
            Role = LlmRole.Assistant,
            Content = gptStreamChoice.Delta.Content,
            StopReason = gptStreamChoice.FinishReason,
        };
    }

    public static LlmContent Map(GptChoice choice)
    {
        return new LlmContent
        {
            Role = MapRole(choice.Message.Role),
            Content = choice.Message.Content,
            StopReason = choice.FinishReason,
        };
    }

    public static LlmUsage Map(GptUsage usage)
    {
        return new LlmUsage
        {
            InputTokens = usage.PromptTokens,
            OutputTokens = usage.CompletionTokens,
        };
    }

    public static LlmModelVersion MapModel(string model)
    {
        return new LlmModelVersion
        {
            Model = model,
        };
    }

    public static LlmRole MapRole(string role)
    {
        var safeRole = role.ToLower().Trim();

        switch (safeRole)
        {
            case "user":
                return LlmRole.User;
            
            case "assistant":
                return LlmRole.Assistant;
            
            default:
                throw new LargeLanguageModelException("Failed to map message role");
        }
    }
}