using System.Net;
using Database;
using Domain;
using Domain.Claims;
using Domain.Exception;
using Domain.OAuth;
using Interface.Factory;
using Interface.Handler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler;

public class SessionHandler : ISessionHandler
{
    private readonly ILogger<SessionHandler> logger;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IOAuthClientFactory oAuthClientFactory;
    private readonly ApplicationContext applicationContext;

    public SessionHandler(
        ILogger<SessionHandler> logger,
        IHttpContextAccessor httpContextAccessor,
        IOAuthClientFactory oAuthClientFactory,
        ApplicationContext applicationContext)
    {
        this.logger = logger;
        this.httpContextAccessor = httpContextAccessor;
        this.oAuthClientFactory = oAuthClientFactory;
        this.applicationContext = applicationContext;
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
            var oAuthRecordId = this.httpContextAccessor.HttpContext!.User.FindFirst(GptClaimKeys.OAuthRecordId)?.Value
                ?? throw new SessionException("oAuthRecordId not found in claims");
            
            var oAuthRecord = this.applicationContext.OAuthRecord.Find(oAuthRecordId)
                ?? throw new SessionException("Could not find OAuthRecord in database");

            var client = this.oAuthClientFactory.Create(oAuthRecord.AuthenticationMethod);
            var oAuthDataConvertible = await client.GetOAuthUserData(oAuthRecord);
            return oAuthDataConvertible.ToOAuthUser();
        }
        catch (Exception e)
        {
            this.logger.LogWarning("GetUserData in SessionHandler threw an exception\n{exception}", e);
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
