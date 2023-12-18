using BusinessLogic.Pipeline.Steam;
using Domain.Entity.Id;
using Domain.Pipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler.OAuth.Steam;

public class SteamOAuthLoginSuccessHandler : BasePipelineExecutorHandler
{
    private readonly ILogger<SteamOAuthLoginSuccessHandler> logger;
    private readonly SteamLoginSuccessPipeline steamLoginSuccessPipeline;

    public SteamOAuthLoginSuccessHandler(
        ILogger<SteamOAuthLoginSuccessHandler> logger,
        SteamLoginSuccessPipeline steamLoginSuccessPipeline)
    {
        this.logger = logger;
        this.steamLoginSuccessPipeline = steamLoginSuccessPipeline;
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
            AuthenticationMethod = Domain.Entity.AuthMethods.Steam,
        };

        var excecutedParametersResult = await this.ExecutePipeline(
            this.logger,
            this.steamLoginSuccessPipeline,
            parameters,
            "SteamLoginRedirect",
            cancellationToken);
        
        return excecutedParametersResult.Match(
            (parameters) => Results.Redirect(parameters.RedirectUri!),
            (error) => Results.StatusCode(500));
    }
}
