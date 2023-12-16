using BusinessLogic.Client;
using BusinessLogic.Pipeline.Github;
using Domain.Entity.Id;
using Domain.Pipeline;
using Interface.Factory;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler.OAuth.Github;

public class GithubOAuthLoginSuccessHandler : BasePipelineExecutorHandler
{
    private readonly ILogger<GithubOAuthLoginSuccessHandler> logger;
    private readonly GithubLoginPipeline githubLoginPipeline;
    private readonly IOAuthClientFactory oAuthClientFactory;

    public GithubOAuthLoginSuccessHandler(
        ILogger<GithubOAuthLoginSuccessHandler> logger,
        GithubLoginPipeline githubLoginPipeline,
        IOAuthClientFactory oAuthClientFactory)
    {
        this.logger = logger;
        this.githubLoginPipeline = githubLoginPipeline;
        this.oAuthClientFactory = oAuthClientFactory;
    }

    public async Task<IResult> LoginSuccess(
        OAuthRecordId oAuthRecordId,
        string code,
        CancellationToken cancellationToken)
    {
        var parameters = new GithubLoginSuccessPipelineParameters
        {
            OAuthRecordId = oAuthRecordId,
            TokenType = "bearer",
            AccessToken = string.Empty,
            Code = code,
            AuthenticationMethod = Domain.Entity.AuthenticationMethod.Development,
        };

        var excecutedParametersResult = await this.ExecutePipeline(
            this.logger,
            this.githubLoginPipeline,
            parameters,
            "GithubLoginSuccess",
            cancellationToken);
        
        return excecutedParametersResult.Match(
            (parameters) => Results.Redirect(parameters.RedirectUri!),
            (error) => Results.StatusCode(500));
    }
}
