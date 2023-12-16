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
        var client = (GithubOAuthClient)this.oAuthClientFactory.Create(Domain.Entity.AuthenticationMethod.Github);
        var codeResponse = await client.ExchangeTheCode(code);

        var parameters = new GithubLoginSuccessPipelineParameters
        {
            OAuthRecordId = oAuthRecordId,
            TokenType = "bearer",
            AccessToken = codeResponse.AccessToken,
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
