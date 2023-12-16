using BusinessLogic.Client;
using BusinessLogic.Pipeline.Github;
using Domain.Entity.Id;
using Domain.Pipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler.OAuth.Github;

public class GithubOAuthLoginSuccessHandler : BasePipelineExecutorHandler
{
    private readonly ILogger<GithubOAuthLoginSuccessHandler> logger;
    private readonly GithubLoginPipeline githubLoginPipeline;

    public GithubOAuthLoginSuccessHandler(
        ILogger<GithubOAuthLoginSuccessHandler> logger,
        GithubLoginPipeline githubLoginPipeline)
    {
        this.logger = logger;
        this.githubLoginPipeline = githubLoginPipeline;
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
