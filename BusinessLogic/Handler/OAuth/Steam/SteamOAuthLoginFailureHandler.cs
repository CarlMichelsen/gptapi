using BusinessLogic.Pipeline.Steam;
using Domain.Entity.Id;
using Domain.Pipeline;
using Interface.Handler.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler.OAuth.Steam;

public class SteamOAuthLoginFailureHandler : BasePipelineExecutorHandler, IOAuthLoginFailureHandler
{
    private readonly ILogger<SteamOAuthLoginFailureHandler> logger;
    private readonly SteamLoginFailurePipeline steamLoginFailurePipeline;

    public SteamOAuthLoginFailureHandler(
        ILogger<SteamOAuthLoginFailureHandler> logger,
        SteamLoginFailurePipeline steamLoginFailurePipeline)
    {
        this.logger = logger;
        this.steamLoginFailurePipeline = steamLoginFailurePipeline;
    }

    public async Task<IResult> LoginFailure(
        OAuthRecordId oAuthRecordId,
        string error,
        CancellationToken cancellationToken)
    {
        var parameters = new LoginFailurePipelineParameters
        {
            OAuthRecordId = oAuthRecordId,
            Error = error,
        };

        var excecutedParametersResult = await this.ExecutePipeline(
            this.logger,
            this.steamLoginFailurePipeline,
            parameters,
            "SteamLoginFailure",
            cancellationToken);
        
        return excecutedParametersResult.Match(
            (parameters) => Results.Redirect(parameters.RedirectUri!),
            (error) => Results.StatusCode(500));
    }
}
