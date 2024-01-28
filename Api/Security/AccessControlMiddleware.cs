using System.Security.Claims;
using Domain;
using Domain.Dto.Session;
using Interface.Service;

namespace Api.Security;

public class AccessControlMiddleware : IMiddleware
{
    private readonly ISessionService sessionService;

    public AccessControlMiddleware(
        ISessionService sessionService)
    {
        this.sessionService = sessionService;
    }

    public async Task InvokeAsync(
        HttpContext context,
        RequestDelegate next)
    {
        var sessionData = await this.sessionService.GetSessionData();
        if (sessionData is not null)
        {
            context.User = this.CreateClaimsPrincipal(sessionData);
            await next(context);
        }
        else
        {
            context.Response.StatusCode = 403;
        }
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
