using Domain.Dto;
using Domain.Dto.LargeLanguageModel;
using Interface.Handler;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Handler;

public class AvailableModelHandler : IAvailableModelHandler
{
    private const string OpenAiProvider = "openai";
    private const string AnthropicProvider = "anthropic";

    private AvailableModelResponse PlaceholderResponse => new AvailableModelResponse
    {
        AvailableModels = new Dictionary<string, List<AvailableModel>>
        {
            {
                OpenAiProvider,
                new List<AvailableModel>
                {
                    new AvailableModel
                    {
                        DisplayName = "GPT 4 Turbo Preview",
                        Description = "The latest GPT-4 model intended to reduce cases of “laziness” where the model doesnt complete a task. Returns a maximum of 4,096 output tokens",
                        ProviderIdentifier = OpenAiProvider,
                        ModelIdentifier = "gpt-4-turbo-preview",
                        MaxTokens = 128000,
                    },
                    new AvailableModel
                    {
                        DisplayName = "GPT 4 Turbo Better Json Preview",
                        Description = "GPT 4 Turbo model featuring improved instruction following, JSON mode, reproducible outputs, parallel function calling, and more. Returns a maximum of 4,096 output tokens. This is a preview model",
                        ProviderIdentifier = OpenAiProvider,
                        ModelIdentifier = "gpt-4-1106-preview",
                        MaxTokens = 128000,
                    },
                    new AvailableModel
                    {
                        DisplayName = "GPT 4 Vision Preview (Vision not supported here yet)",
                        Description = "GPT 4 with the ability to understand images, in addition to all other GPT-4 Turbo capabilities. Currently points to gpt-4-1106-vision-preview",
                        ProviderIdentifier = OpenAiProvider,
                        ModelIdentifier = "gpt-4-vision-preview",
                        MaxTokens = 128000,
                    },
                    new AvailableModel
                    {
                        DisplayName = "GPT 4",
                        Description = "Snapshot of gpt-4 from June 13th 2023 with improved function calling support",
                        ProviderIdentifier = OpenAiProvider,
                        ModelIdentifier = "gpt-4",
                        MaxTokens = 8192,
                    },
                    new AvailableModel
                    {
                        DisplayName = "GPT 4 32K",
                        Description = "Snapshot of gpt-4-32k from June 13th 2023 with improved function calling support. This model was never rolled out widely in favor of GPT-4 Turbo",
                        ProviderIdentifier = OpenAiProvider,
                        ModelIdentifier = "gpt-4-32k",
                        MaxTokens = 32768,
                    },
                    new AvailableModel
                    {
                        DisplayName = "GPT 3.5 Turbo",
                        Description = "The latest GPT-3.5 Turbo model with higher accuracy at responding in requested formats and a fix for a bug which caused a text encoding issue for non-English language function calls. Returns a maximum of 4,096 output tokens",
                        ProviderIdentifier = OpenAiProvider,
                        ModelIdentifier = "gpt-3.5-turbo",
                        MaxTokens = 16385,
                    },
                }
            },
            {
                AnthropicProvider,
                new List<AvailableModel>
                {
                    new AvailableModel
                    {
                        DisplayName = "Claude 3 Opus",
                        Description = "Most powerful model for highly complex tasks",
                        ProviderIdentifier = AnthropicProvider,
                        ModelIdentifier = "claude-3-opus-20240229",
                        MaxTokens = 4096,
                    },
                    new AvailableModel
                    {
                        DisplayName = "Claude 3 Sonnet",
                        Description = "Ideal balance of intelligence and speed for enterprise workloads",
                        ProviderIdentifier = AnthropicProvider,
                        ModelIdentifier = "claude-3-sonnet-20240229",
                        MaxTokens = 4096,
                    },
                    new AvailableModel
                    {
                        DisplayName = "Claude 3 Haiku",
                        Description = "Fastest and most compact model for near-instant responsiveness",
                        ProviderIdentifier = AnthropicProvider,
                        ModelIdentifier = "claude-3-haiku-20240307",
                        MaxTokens = 4096,
                    },
                }
            },
        },
    };
    
    public Task<IResult> GetAvailableModels()
    {
        return Task.FromResult(Results.Ok(new ServiceResponse<AvailableModelResponse>(this.PlaceholderResponse)));
    }
}