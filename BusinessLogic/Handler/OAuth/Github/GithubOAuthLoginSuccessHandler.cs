using BusinessLogic.Client;
using BusinessLogic.Pipeline.Github;
using Domain;
using Domain.Entity.Id;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Factory;
using Interface.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler.OAuth.Github;

public class GithubOAuthLoginSuccessHandler : BasePipelineExecutorHandler
{
    private readonly ILogger<GithubOAuthLoginSuccessHandler> logger;
    private readonly GithubLoginPipeline githubLoginPipeline;
    private readonly IOAuthClientFactory oAuthClientFactory;
    private readonly IEndpointUrlProvider endpointUrlProvider;

    public GithubOAuthLoginSuccessHandler(
        ILogger<GithubOAuthLoginSuccessHandler> logger,
        GithubLoginPipeline githubLoginPipeline,
        IOAuthClientFactory oAuthClientFactory,
        IEndpointUrlProvider endpointUrlProvider)
    {
        this.logger = logger;
        this.githubLoginPipeline = githubLoginPipeline;
        this.oAuthClientFactory = oAuthClientFactory;
        this.endpointUrlProvider = endpointUrlProvider;
    }

    public async Task<IResult> LoginSuccess(
        OAuthRecordId oAuthRecordId,
        string code,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = (GithubOAuthClient)this.oAuthClientFactory.Create(Domain.Entity.AuthenticationMethod.Github);
            var codeResponse = await client.ExchangeTheCode(code);

            var parameters = new GithubLoginSuccessPipelineParameters
            {
                OAuthRecordId = oAuthRecordId,
                TokenType = codeResponse.TokenType,
                AccessToken = codeResponse.AccessToken,
                CommaSeparatedScopes = codeResponse.CommaSeparatedScopes,
                Code = code,
                AuthenticationMethod = Domain.Entity.AuthenticationMethod.Github,
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
        catch (Exception e)
        {
            this.logger.LogCritical(
                "A critical unhandled exception occured in {handlername}:\n{exception}\nredirecting...",
                nameof(GithubOAuthLoginSuccessHandler),
                e);
            
            var redirectUrl = this.endpointUrlProvider.GetEndpointUrlFromEndpointName(GptApiConstants.FrontendEndpointName);
            return Results.Redirect(redirectUrl);
        }
    }
}
