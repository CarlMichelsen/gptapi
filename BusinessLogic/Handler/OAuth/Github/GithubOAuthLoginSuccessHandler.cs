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
        string tokenType,
        string accessToken,
        CancellationToken cancellationToken)
    {
        var parameters = new GithubLoginSuccessPipelineParameters
        {
            OAuthRecordId = oAuthRecordId,
            TokenType = tokenType,
            AccessToken = accessToken,
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
