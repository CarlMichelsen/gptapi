using Domain.Exception;
using Domain.LargeLanguageModel.Shared;

namespace Domain.LargeLanguageModel.OpenAi.Map;

public static class GptMapper
{
    public static LargeLanguageModelResponse Map(GptChatResponse response)
    {
        return new LargeLanguageModelResponse
        {
            Id = response.Id,
            Object = response.Object,
            Created = response.Created,
            ModelName = response.Model,
            ResponseFingerprint = response.SystemFingerprint,
            Options = response.Choices.Select(Map).ToList(),
        };
    }

    public static LargeLanguageModelChunk Map(GptChatStreamChunk chunk)
    {
        return new LargeLanguageModelChunk
        {
            Id = chunk.Id,
            Object = chunk.Object,
            Created = chunk.Created,
            ModelName = chunk.Model,
            ResponseFingerprint = chunk.SystemFingerprint,
            Options = chunk.Choices.Select(Map).ToList(),
        };
    }

    public static LargeLanguageModelOption Map(GptChoice choice)
    {
        return new LargeLanguageModelOption
        {
            Index = choice.Index,
            FinishReason = choice.FinishReason,
            Message = Map(choice.Message),
        };
    }

    public static LargeLanguageModelStreamOption Map(GptStreamChoice choice)
    {
        return new LargeLanguageModelStreamOption
        {
            Index = choice.Index,
            FinishReason = choice.FinishReason,
            Message = Map(choice.Delta),
        };
    }

    public static LargeLanguageModelDelta Map(GptDelta delta)
    {
        return new LargeLanguageModelDelta
        {
            Content = delta.Content,
        };
    }

    public static LargeLanguageModelMessage Map(GptChatMessage chatMessage)
    {
        return new LargeLanguageModelMessage
        {
            Role = Map(chatMessage.Role),
            Content = chatMessage.Content,
        };
    }

    public static LargeLanguageModelMessage Map(GptReceivedMessage message)
    {
        return new LargeLanguageModelMessage
        {
            Role = Map(message.Role),
            Content = message.Content,
        };
    }

    public static LargeLanguageModelMessageRole Map(string role)
    {
        var safeRole = role.ToLower().Trim();

        switch (safeRole)
        {
            case "user":
                return LargeLanguageModelMessageRole.User;
            
            case "assistant":
                return LargeLanguageModelMessageRole.Assistant;
            
            case "system":
                return LargeLanguageModelMessageRole.System;
            
            default:
                throw new LargeLanguageModelException("Failed to map message role");
        }
    }
}