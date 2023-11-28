using BusinessLogic.Pipeline;
using Domain;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Handler;
using Interface.Pipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler;

public class SteamOAuthHandler2 : ISteamOAuthHandler
{
    private readonly ILogger<SteamOAuthHandler2> logger;
    private readonly LoginStartPipeline loginStartPipeline;
    private readonly LoginFailurePipeline loginFailurePipeline;
    private readonly LoginSuccessPipeline loginSuccessPipeline;

    public SteamOAuthHandler2(
        ILogger<SteamOAuthHandler2> logger,
        LoginStartPipeline startLoginProcessPipeline,
        LoginFailurePipeline loginFailurePipeline,
        LoginSuccessPipeline loginSuccessPipeline)
    {
        this.logger = logger;
        this.loginStartPipeline = startLoginProcessPipeline;
        this.loginFailurePipeline = loginFailurePipeline;
        this.loginSuccessPipeline = loginSuccessPipeline;
    }

    public async Task<IResult> SteamLogin()
    {
        // get cancellation token from somewhere that matters...
        var source = new CancellationTokenSource();

        var parameters = new LoginStartPipelineParameters
        {
            OAuthRecordId = Guid.NewGuid(),
        };

        var excecutedParametersResult = await this.ExecutePipeline(
            this.loginStartPipeline,
            parameters,
            "SteamLogin",
            source.Token);
        
        return excecutedParametersResult.Match(
            (parameters) => Results.Redirect(parameters.RedirectUri!),
            (_) => Results.StatusCode(500));
    }

    public async Task<IResult> SteamLoginFailure(
        Guid oAuthRecordId,
        string error)
    {
        // get cancellation token from somewhere that matters...
        var source = new CancellationTokenSource();

        var parameters = new LoginFailurePipelineParameters
        {
            OAuthRecordId = oAuthRecordId,
            Error = error,
        };

        var excecutedParametersResult = await this.ExecutePipeline(
            this.loginFailurePipeline,
            parameters,
            "SteamLoginFailure",
            source.Token);
        
        return excecutedParametersResult.Match(
            (parameters) => Results.Redirect(parameters.RedirectUri!),
            (error) => Results.StatusCode(500));
    }

    public async Task<IResult> SteamLoginSuccess(Guid oAuthRecordId, string tokenType, string accessToken)
    {
        // get cancellation token from somewhere that matters...
        var source = new CancellationTokenSource();

        var parameters = new LoginSuccessPipelineParameters
        {
            OAuthRecordId = oAuthRecordId,
            TokenType = tokenType,
            AccessToken = accessToken,
        };

        var excecutedParametersResult = await this.ExecutePipeline(
            this.loginSuccessPipeline,
            parameters,
            "SteamLoginSuccess",
            source.Token);
        
        return excecutedParametersResult.Match(
            (parameters) => 
            {
                this.logger.LogCritical("SteamLoginSuccess redirect uri {endpoint}", parameters.RedirectUri);
                return parameters.Authorized ? Results.Redirect(parameters.RedirectUri!) : Results.Unauthorized();
            },
            (error) => Results.StatusCode(500));

        throw new NotImplementedException();
    }

    private async Task<Result<T, string>> ExecutePipeline<T>(
        IPipeline<T> pipeline,
        T parameters,
        string methodName,
        CancellationToken cancellationToken)
    {
        try
        {
            return await pipeline.Execute(parameters, cancellationToken);
        }
        catch (PipelineException e)
        {
            this.logger.LogError(
                "A PipelineException happened in the {method} method {e}",
                methodName,
                e);
        }
        catch (Exception e)
        {
            this.logger.LogCritical(
                "A critical exception happened in the {method} method {e}",
                methodName,
                e);
        }

        return "failure";
    }
}