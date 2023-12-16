using System.Net;
using Domain;
using Domain.Claims;
using Domain.Dto.Steam;
using Domain.Entity;
using Domain.Exception;
using Domain.OAuth;
using Interface.Factory;
using Interface.Handler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Handler;

public class SessionHandler : ISessionHandler
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IOAuthClientFactory oAuthClientFactory;

    public SessionHandler(
        IHttpContextAccessor httpContextAccessor,
        IOAuthClientFactory oAuthClientFactory)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.oAuthClientFactory = oAuthClientFactory;
    }

    public async Task<Result<OAuthUserData, HttpStatusCode>> GetUserData()
    {
        if (this.httpContextAccessor.HttpContext!.User.Identity?.IsAuthenticated != true)
        {
            // This is a redundant check. Better safe than sorry tho.
            return HttpStatusCode.Unauthorized;
        }

        try
        {
            var authenticationId = this.httpContextAccessor.HttpContext!.User.FindFirst(GptClaimKeys.AuthenticationId)?.Value
                ?? throw new SessionException("authenticationId not found in claims");

            var authenticationMethodString = this.httpContextAccessor.HttpContext!.User.FindFirst(GptClaimKeys.AuthenticationMethod)?.Value
                ?? throw new SessionException("authenticationMethodString not found in claims");
            
            if (!Enum.TryParse(authenticationMethodString, out AuthenticationMethod authenticationMethod))
            {
                throw new SessionException($"authenticationMethodString ({authenticationMethodString}) not valid");
            }

            var client = this.oAuthClientFactory.Create(authenticationMethod);
            var oAuthDataConvertible = await client.GetOAuthUserData(authenticationId);
            return oAuthDataConvertible.ToOAuthUser();
        }
        catch (Exception)
        {
            return HttpStatusCode.InternalServerError;
        }
    }

    public async Task<Result<bool, HttpStatusCode>> Logout()
    {
        try
        {
            await this.httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return true;
        }
        catch (Exception)
        {
            return HttpStatusCode.InternalServerError;
        }
    }
}
