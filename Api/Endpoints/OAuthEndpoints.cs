using System.Runtime.CompilerServices;
using BusinessLogic.Handler.OAuth.Development;
using BusinessLogic.Handler.OAuth.Github;
using BusinessLogic.Handler.OAuth.Steam;
using Domain;
using Domain.Entity.Id;
using Interface.Handler.OAuth;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class OAuthEndpoints
{
    public static RouteGroupBuilder MapSteamOAuthEndpoints(this RouteGroupBuilder group)
    {
        var oAuthGroup = group.MapGroup("/oauth");

        oAuthGroup.MapGet(
            "/DevelopmentLogin",
            async ([FromServices] DevelopmentOAuthLoginHandler developmentOAuthLoginHandler) => await developmentOAuthLoginHandler.Login())
            .WithName("DevelopmentLogin");

        oAuthGroup.MapGet(
            "/GithubLogin",
            async ([FromServices] GithubOAuthLoginHandler githubOAuthLoginHandler) => await githubOAuthLoginHandler.Login())
            .WithName("GithubLogin");

        oAuthGroup.MapGet(
            "/SteamLogin",
            async ([FromServices] SteamOAuthLoginHandler steamOAuthLoginHandler) => await steamOAuthLoginHandler.Login())
            .WithName("SteamLogin");

        oAuthGroup.MapGet(
            "/DevelopmentLoginSuccess",
            async (
                [FromServices] DevelopmentOAuthLoginSuccessHandler devlopmentOAuthLoginSuccessHandler,
                [FromQuery(Name = "access_token")] string accessToken,
                [FromQuery(Name = "token_type")] string tokenType,
                [FromQuery(Name = "state")] Guid oAuthRecordId,
                CancellationToken cancellationToken) => 
            {
                return await devlopmentOAuthLoginSuccessHandler
                    .LoginSuccess(new OAuthRecordId(oAuthRecordId), tokenType, accessToken, cancellationToken);
            })
            .WithName(GptApiConstants.DevelopmentLoginSuccessEndPointName);

        oAuthGroup.MapGet(
            "/GithubLoginRedirect",
            async (
                [FromServices] ILogger<GithubOAuthLoginSuccessHandler> debugLogger,
                [FromServices] GithubOAuthLoginSuccessHandler githubOAuthLoginSuccessHandler,
                [FromQuery(Name = "code")] string code,
                [FromQuery(Name = "state")] Guid oAuthRecordId,
                CancellationToken cancellationToken) => 
            {
                return await githubOAuthLoginSuccessHandler
                    .LoginSuccess(new OAuthRecordId(oAuthRecordId), code, cancellationToken);
            })
            .WithName(GptApiConstants.GithubLoginRedirectEndPointName);

        oAuthGroup.MapGet(
            "/SteamLoginRedirect",
            async (
                [FromServices] SteamOAuthLoginSuccessHandler steamOAuthLoginSuccessHandler,
                [FromQuery(Name = "access_token")] string accessToken,
                [FromQuery(Name = "token_type")] string tokenType,
                [FromQuery(Name = "state")] Guid oAuthRecordId,
                CancellationToken cancellationToken) => 
            {
                return await steamOAuthLoginSuccessHandler
                    .LoginSuccess(new OAuthRecordId(oAuthRecordId), tokenType, accessToken, cancellationToken);
            })
            .WithName("SteamLoginRedirect");

        oAuthGroup.MapGet(
            "/SteamLoginFailure",
            async (
                [FromServices] IOAuthLoginFailureHandler steamOAuthLoginFailureHandler,
                [FromQuery(Name = "error")] string error,
                [FromQuery(Name = "state")] Guid oAuthRecordId,
                CancellationToken cancellationToken) => 
            {
                return await steamOAuthLoginFailureHandler
                    .LoginFailure(new OAuthRecordId(oAuthRecordId), error, cancellationToken);
            })
            .WithName("SteamLoginFailure");
        return group;
    }
}
