using BusinessLogic.Pipeline.Steam;
using Domain.Entity.Id;
using Domain.Pipeline;
using Interface.Handler.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler.OAuth.Steam;

public class SteamOAuthLoginSuccessHandler : BasePipelineExecutorHandler, IOAuthLoginSuccessHandler
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
        var parameters = new LoginSuccessPipelineParameters
        {
            OAuthRecordId = oAuthRecordId,
            TokenType = tokenType,
            AccessToken = accessToken,
        };

        var excecutedParametersResult = await this.ExecutePipeline(
            this.logger,
            this.steamLoginSuccessPipeline,
            parameters,
            "SteamLoginSuccess",
            cancellationToken);
        
        return excecutedParametersResult.Match(
            (parameters) => Results.Redirect(parameters.RedirectUri!),
            (error) => Results.StatusCode(500));
    }
}
