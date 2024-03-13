using Domain.Exception;
using Domain.LargeLanguageModel.Shared.Request;
using Domain.LargeLanguageModel.Shared.Response;

namespace Domain.LargeLanguageModel.Claude.Map;

public static class ClaudeResponseMapper
{
    public static LlmResponse Map(ClaudeResponse claudeResponse)
    {
        return new LlmResponse
        {
            Id = claudeResponse.Id,
            Model = MapModel(claudeResponse.Model),
            Choices = claudeResponse.Content.Select((c) => Map(c, claudeResponse.Role, claudeResponse.StopReason)).ToList(),
            Usage = Map(claudeResponse.Usage),
        };
    }

    public static LlmModelVersion MapModel(string model)
    {
        return new LlmModelVersion
        {
            Model = model,
        };
    }

    public static LlmUsage Map(ClaudeUsage claudeUsage)
    {
        return new LlmUsage
        {
            InputTokens = claudeUsage.InputTokens,
            OutputTokens = claudeUsage.OutputTokens,
        };
    }

    public static LlmContent Map(
        ClaudeResponseContent claudeResponseContent,
        string claudeRole,
        string? stopReason)
    {
        return new LlmContent
        {
            Role = MapRole(claudeRole),
            Content = claudeResponseContent.Text,
            StopReason = stopReason,
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
