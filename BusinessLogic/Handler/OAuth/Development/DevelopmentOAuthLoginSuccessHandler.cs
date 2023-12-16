using BusinessLogic.Pipeline.Development;
using Domain.Entity.Id;
using Domain.Pipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler.OAuth.Development;

public class DevelopmentOAuthLoginSuccessHandler : BasePipelineExecutorHandler
{
    private readonly ILogger<DevelopmentOAuthLoginSuccessHandler> logger;
    private readonly DevelopmentLoginPipeline developmentLoginPipeline;

    public DevelopmentOAuthLoginSuccessHandler(
        ILogger<DevelopmentOAuthLoginSuccessHandler> logger,
        DevelopmentLoginPipeline developmentLoginPipeline)
    {
        this.logger = logger;
        this.developmentLoginPipeline = developmentLoginPipeline;
    }

    public async Task<IResult> LoginSuccess(
        OAuthRecordId oAuthRecordId,
        string tokenType,
        string accessToken,
        CancellationToken cancellationToken)
    {
        var parameters = new SteamLoginSuccessPipelineParameters
        {
            OAuthRecordId = oAuthRecordId,
            TokenType = tokenType,
            AccessToken = accessToken,
            AuthenticationMethod = Domain.Entity.AuthenticationMethod.Development,
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
}
