using Domain;
using Domain.Configuration;
using Domain.Dto.Discord;
using Domain.Pipeline;
using Interface.Client;
using Interface.Pipeline;
using Interface.Provider;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Pipeline.Shared;

public class DeriveSuccessRedirectUriStage : IPipelineStage<ILoginPipelineParameters>
{
    private readonly ILogger<DeriveSuccessRedirectUriStage> logger;
    private readonly IOptions<ApplicationOptions> applicationOptions;
    private readonly IEndpointUrlProvider endpointUrlProvider;
    private readonly IDiscordMessageClient discordMessageClient;

    public DeriveSuccessRedirectUriStage(
        ILogger<DeriveSuccessRedirectUriStage> logger,
        IOptions<ApplicationOptions> applicationOptions,
        IEndpointUrlProvider endpointUrlProvider,
        IDiscordMessageClient discordMessageClient)
    {
        this.logger = logger;
        this.applicationOptions = applicationOptions;
        this.endpointUrlProvider = endpointUrlProvider;
        this.discordMessageClient = discordMessageClient;
    }

    public async Task<ILoginPipelineParameters> Process(
        ILoginPipelineParameters input,
        CancellationToken cancellationToken)
    {
        if (this.applicationOptions.Value.IsDevelopment)
        {
            input.RedirectUri = GptApiConstants.DeveloperFrontendUrl;
            await this.LogLogin(input);
            return input;
        }
        
        var uri = this.endpointUrlProvider.GetEndpointUrlFromEndpointName(GptApiConstants.FrontendEndpointName);

        input.RedirectUri = uri;
        await this.LogLogin(input);
        return input;
    }

    private async Task LogLogin(ILoginPipelineParameters input)
    {
        var message = new DiscordWebhookMessage
        {
            Content = $"A user ({input.UserProfileId}) logged in using {Enum.GetName(input.AuthenticationMethod)} OAuth",
        };

        var logged = await this.discordMessageClient.SendMessage(message);
        if (!logged)
        {
            this.logger.LogCritical("Failed to log to DISCORD");
        }
    }
}
