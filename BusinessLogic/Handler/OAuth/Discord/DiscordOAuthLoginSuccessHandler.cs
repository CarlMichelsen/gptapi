using BusinessLogic.Client;
using BusinessLogic.Pipeline.Discord;
using Domain;
using Domain.Entity.Id;
using Domain.Pipeline;
using Interface.Factory;
using Interface.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler.OAuth.Discord;

public class DiscordOAuthLoginSuccessHandler : BasePipelineExecutorHandler
{
    private readonly ILogger<DiscordOAuthLoginSuccessHandler> logger;
    private readonly DiscordLoginPipeline discordLoginPipeline;
    private readonly IOAuthClientFactory oAuthClientFactory;
    private readonly IEndpointUrlProvider endpointUrlProvider;

    public DiscordOAuthLoginSuccessHandler(
        ILogger<DiscordOAuthLoginSuccessHandler> logger,
        DiscordLoginPipeline discordLoginPipeline,
        IOAuthClientFactory oAuthClientFactory,
        IEndpointUrlProvider endpointUrlProvider)
    {
        this.logger = logger;
        this.discordLoginPipeline = discordLoginPipeline;
        this.oAuthClientFactory = oAuthClientFactory;
        this.endpointUrlProvider = endpointUrlProvider;
    }

    public async Task<IResult> LoginSuccess(
        OAuthRecordId oAuthRecordId,
        string tokenType,
        string code,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = (DiscordOAuthClient)this.oAuthClientFactory.Create(Domain.Entity.AuthMethods.Github);
            var codeResponse = await client.ExchangeTheCode(code);

            return Results.Ok();

            /*var parameters = new DiscordLoginSuccessPipelineParameters
            {
                OAuthRecordId = oAuthRecordId,
                TokenType = codeResponse.TokenType,
                AccessToken = codeResponse.AccessToken,
                Scopes = codeResponse.CommaSeparatedScopes,
                Code = code,
                AuthenticationMethod = Domain.Entity.AuthMethods.Github,
            };

            var excecutedParametersResult = await this.ExecutePipeline(
                this.logger,
                this.discordLoginPipeline,
                parameters,
                "DiscordLoginSuccess",
                cancellationToken);
            
            return excecutedParametersResult.Match(
                (parameters) => Results.Redirect(parameters.RedirectUri!),
                (error) => Results.StatusCode(500));*/
        }
        catch (Exception e)
        {
            this.logger.LogCritical(
                "A critical unhandled exception occured in {handlername}:\n{exception}\nredirecting...",
                nameof(DiscordOAuthLoginSuccessHandler),
                e);
            
            var redirectUrl = this.endpointUrlProvider.GetEndpointUrlFromEndpointName(GptApiConstants.FrontendEndpointName);
            return Results.Redirect(redirectUrl);
        }
    }
}
