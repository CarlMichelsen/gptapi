using BusinessLogic.Json;
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

    public async Task<SessionData?> GetSessionData()
    {
        try
        {
            var httpContext = this.httpContextAccessor.HttpContext;
            if (httpContext is null)
            {
                return default;
            }

            var cookieDataString = httpContext.Request.Cookies["Access"];
            if (cookieDataString is null)
            {
                return default;
            }

            var cookieData = CamelCaseJsonParser.Deserialize<CookieData>(cookieDataString);
            if (cookieData is null)
            {
                return default;
            }

            var sessionDataString = await this.cacheService
                .Get(cookieData.SessionCacheKey);

            return CamelCaseJsonParser.Deserialize<SessionData>(sessionDataString);
        }
        catch (System.Exception e)
        {
            this.logger.LogCritical(
                "An exception occured when attempting to fetch sessionData {exception}",
                e);
            return default;
        }
    }
}
