using BusinessLogic.Pipeline.Development;
using Domain;
using Domain.Entity.Id;
using Domain.Pipeline;
using Interface.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler.OAuth.Development;

public class DevelopmentOAuthLoginSuccessHandler : BasePipelineExecutorHandler
{
    private readonly ILogger<DevelopmentOAuthLoginSuccessHandler> logger;
    private readonly DevelopmentLoginPipeline developmentLoginPipeline;
    private readonly IEndpointUrlProvider endpointUrlProvider;

    public DevelopmentOAuthLoginSuccessHandler(
        ILogger<DevelopmentOAuthLoginSuccessHandler> logger,
        DevelopmentLoginPipeline developmentLoginPipeline,
        IEndpointUrlProvider endpointUrlProvider)
    {
        this.logger = logger;
        this.developmentLoginPipeline = developmentLoginPipeline;
        this.endpointUrlProvider = endpointUrlProvider;
    }

    public async Task<IResult> LoginSuccess(
        OAuthRecordId oAuthRecordId,
        string tokenType,
        string accessToken,
        CancellationToken cancellationToken)
    {
        try
        {
            var parameters = new DevelopmentLoginPipelineParameters
            {
                OAuthRecordId = oAuthRecordId,
                TokenType = tokenType,
                AccessToken = accessToken,
                AuthenticationMethod = Domain.Entity.AuthMethods.Development,
            };

            var excecutedParametersResult = await this.ExecutePipeline(
                this.logger,
                this.developmentLoginPipeline,
                parameters,
                "DevelopmentLoginSuccess",
                cancellationToken);
            
            return excecutedParametersResult.Match(
                (parameters) => Results.Redirect(parameters.RedirectUri!),
                (error) => Results.StatusCode(500));
        }
        catch (Exception e)
        {
            this.logger.LogCritical(
                "A critical unhandled exception occured in {handlername}:\n{exception}\nredirecting...",
                nameof(DevelopmentOAuthLoginSuccessHandler),
                e);
            
            var redirectUrl = this.endpointUrlProvider.GetEndpointUrlFromEndpointName(GptApiConstants.FrontendEndpointName);
            return Results.Redirect(redirectUrl);
        }
    }
}
