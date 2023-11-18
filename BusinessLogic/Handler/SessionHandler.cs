using System.Net;
using Domain;
using Domain.Dto.Steam;
using Domain.Exception;
using Interface.Factory;
using Interface.Handler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Handler;

public class SessionHandler : ISessionHandler
{
    private readonly ISteamClientFactory steamClientFactory;

    public SessionHandler(
        ISteamClientFactory steamClientFactory)
    {
        this.steamClientFactory = steamClientFactory;
    }

    public async Task<Result<SteamPlayerDto, HttpStatusCode>> GetUserData(
        HttpContext httpContext)
    {
        if (httpContext.User.Identity?.IsAuthenticated != true)
        {
            // This is a redundant check. Better safe than sorry tho.
            return HttpStatusCode.Unauthorized;
        }

        try
        {
            var steamId = httpContext.User.FindFirst("SteamId")?.Value
            ?? throw new SessionException("SteamId not found in claims!");

            var client = this.steamClientFactory.Create();
            return await client.GetSteamPlayerSummary(steamId);
        }
        catch (Exception)
        {
            return HttpStatusCode.InternalServerError;
        }
    }

    public async Task<Result<bool, HttpStatusCode>> Logout(HttpContext httpContext)
    {
        try
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return true;
        }
        catch (Exception)
        {
            return HttpStatusCode.InternalServerError;
        }
    }
}
