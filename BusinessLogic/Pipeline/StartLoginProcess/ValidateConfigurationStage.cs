using Domain.Configuration;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Pipeline;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Pipeline.StartLoginProcess;

public class ValidateConfigurationStage : IPipelineStage<StartLoginPipelineParameters>
{
    private readonly IOptions<SteamOAuthOptions> steamOAuthOptions;

    public ValidateConfigurationStage(
        IOptions<SteamOAuthOptions> steamOAuthOptions)
    {
        this.steamOAuthOptions = steamOAuthOptions;
    }

    public Task<StartLoginPipelineParameters> Process(
        StartLoginPipelineParameters input,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(this.steamOAuthOptions.Value.ClientId))
        {
            throw new PipelineException("Steam OAuth ClientId is not configured");
        }

        return Task.FromResult(input);
    }
}
