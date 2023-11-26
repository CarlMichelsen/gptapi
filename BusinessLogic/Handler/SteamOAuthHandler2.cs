using BusinessLogic.Pipeline.StartLoginProcess;
using Domain;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Handler;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler;

public class SteamOAuthHandler2 : ISteamOAuthHandler
{
    private readonly ILogger<SteamOAuthHandler2> logger;
    private readonly StartLoginProcessPipeline startLoginProcessPipeline;

    public SteamOAuthHandler2(
        ILogger<SteamOAuthHandler2> logger,
        StartLoginProcessPipeline startLoginProcessPipeline)
    {
        this.logger = logger;
        this.startLoginProcessPipeline = startLoginProcessPipeline;
    }

    public async Task<IResult> SteamLogin()
    {
        // get cancellation token from somewhere that matters...
        var source = new CancellationTokenSource();

        var parameters = new StartLoginPipelineParameters
        {
            OAuthRecordId = Guid.NewGuid(),
        };

        try
        {
            var modifiedParameters = await this.startLoginProcessPipeline.Execute(
                parameters,
                source.Token);
            
            return Results.Redirect(modifiedParameters.RedirectUri!);
        }
        catch (PipelineException e)
        {
            this.logger.LogError(
                "A PipelineException happened in the SteamLogin method {e}",
                e);
        }
        catch (Exception e)
        {
            this.logger.LogCritical(
                "A critical exception happened in the SteamLogin method {e}",
                e);
        }

        return Results.StatusCode(500);
    }

    public Task<IResult> SteamLoginFailure(Guid oAuthRecordId, string error)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> SteamLoginSuccess(Guid oAuthRecordId, string tokenType, string accessToken)
    {
        throw new NotImplementedException();
    }
}