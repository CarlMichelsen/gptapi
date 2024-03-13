using Domain.Exception;
using Domain.LargeLanguageModel.Shared;

namespace Domain.LargeLanguageModel.Claude.Map;

public static class ClaudeResponseMapper
{
    public static LargeLanguageModelResponse Map(ClaudeResponse claudeResponse)
    {
        return new LargeLanguageModelResponse
        {
            Id = claudeResponse.Id,
            Object = string.Empty,
            Created = DateTime.UtcNow,
            ModelName = claudeResponse.Model,
            Options = new List<LargeLanguageModelOption>
            {
                MapOption(claudeResponse),
            },
        };
    }

    public static LargeLanguageModelOption MapOption(ClaudeResponse claudeResponse)
    {
        return new LargeLanguageModelOption
        {
            Index = 0,
            FinishReason = claudeResponse.StopReason,
            Message = MapMessage(claudeResponse),
        };
    }

    public static LargeLanguageModelMessage MapMessage(ClaudeResponse claudeResponse)
    {
        return new LargeLanguageModelMessage
        {
            Role = Map(claudeResponse.Role),
            Content = claudeResponse.Content.First().Text,
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
