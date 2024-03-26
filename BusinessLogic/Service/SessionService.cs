using BusinessLogic.Json;
using Domain.Abstractions;
using Domain.Dto.Session;
using Interface.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Service;

public class SessionService : ISessionService
{
    private readonly ILogger<SessionService> logger;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ICacheService cacheService;

    public SessionService(
        ILogger<SessionService> logger,
        IHttpContextAccessor httpContextAccessor,
        ICacheService cacheService)
    {
        this.logger = logger;
        this.httpContextAccessor = httpContextAccessor;
        this.cacheService = cacheService;
    }

    public async Task<Result<SessionData>> GetSessionData()
    {
        try
        {
            var httpContext = this.httpContextAccessor.HttpContext;
            if (httpContext is null)
            {
                return new Error("GetSessionData.NoHttpContext");
            }

            var cookieDataString = httpContext.Request.Cookies["Access"];
            if (cookieDataString is null)
            {
                return new Error("GetSessionData.NoAccessCookie");
            }

            var cookieData = CamelCaseJsonParser.Deserialize<CookieData>(cookieDataString);
            if (cookieData is null)
            {
                return new Error("GetSessionData.UnserializableAccessCookie");
            }

            var sessionDataString = await this.cacheService
                .Get(cookieData.SessionCacheKey);
            
            if (string.IsNullOrWhiteSpace(sessionDataString))
            {
                return new Error("GetSessionData.NoSessionData");
            }
            
            var sessionData = CamelCaseJsonParser.Deserialize<SessionData>(sessionDataString);
            if (sessionData is null)
            {
                return new Error("GetSessionData.UnserializableSessionData");
            }

            return sessionData;
        }
        catch (Exception e)
        {
            this.logger.LogCritical(
                "An exception occured when attempting to fetch sessionData {exception}",
                e);
            
            return new Error("GetSessionData.Exception");
        }
    }
}
