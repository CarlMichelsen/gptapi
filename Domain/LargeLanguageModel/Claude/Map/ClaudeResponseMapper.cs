using Domain.Exception;
using Domain.LargeLanguageModel.Claude.Stream;
using Domain.LargeLanguageModel.Claude.Stream.Event;
using Domain.LargeLanguageModel.Shared.Request;
using Domain.LargeLanguageModel.Shared.Response;
using Domain.LargeLanguageModel.Shared.Stream;

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

    public static LlmChunk Map(EventContentBlockDelta eventContentBlockDelta)
    {
        return new LlmChunk
        {
            Choices = new List<LlmContent> { Map(eventContentBlockDelta.Delta) },
        };
    }

    public static LlmContent Map(ClaudeContentBlock claudeContentBlock)
    {
        if (claudeContentBlock.Type != "text_delta")
        {
            throw new LargeLanguageModelException($"Unsupported ClaudeContentBlock Type {claudeContentBlock.Type}");
        }

        return new LlmContent
        {
            Role = LlmRole.Assistant,
            Content = claudeContentBlock.Text,
            StopReason = null,
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
