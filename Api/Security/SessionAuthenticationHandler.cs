using System.Security.Claims;
using System.Text.Encodings.Web;
using Domain;
using Domain.Dto.Session;
using Interface.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Api.Security;

public class SessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ISessionService sessionService;

    public SessionAuthenticationHandler(
        ISessionService sessionService,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
        this.sessionService = sessionService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var endpoint = this.Context.GetEndpoint();
        var authorizeData = endpoint?.Metadata?.GetMetadata<IAuthorizeData>();

        if (authorizeData is null || authorizeData.AuthenticationSchemes?.Any() == false)
        {
            // If the endpoint doesn't require authorization, skip authentication
            return AuthenticateResult.NoResult();
        }

        var sessionDataResult = await this.sessionService.GetSessionData();
        if (sessionDataResult.IsSuccess)
        {
            var claimsPrincipal = this.CreateClaimsPrincipal(sessionDataResult.Unwrap());
            var ticket = new AuthenticationTicket(claimsPrincipal, GptApiConstants.SessionAuthenticationScheme);
            return AuthenticateResult.Success(ticket);
        }
        
        return AuthenticateResult.NoResult();
    }

    private ClaimsPrincipal CreateClaimsPrincipal(
        SessionData sessionData)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimsConstants.UserProfileId, sessionData.UserProfileId.ToString()),
            new Claim(ClaimsConstants.AuthenticationMethodUserId, sessionData.User.Id),
            new Claim(ClaimsConstants.Name, sessionData.User.Name),
            new Claim(ClaimsConstants.Email, sessionData.User.Email),
            new Claim(ClaimsConstants.AuthenticationMethod, sessionData.AuthenticationMethod.ToString()),
            new Claim(ClaimsConstants.AuthenticationMethodName, sessionData.AuthenticationMethodName),
        };

        var identity = new ClaimsIdentity(claims, "session");
        return new ClaimsPrincipal(identity);
    }
}
