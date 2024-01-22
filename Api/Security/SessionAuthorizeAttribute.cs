using System.Security.Claims;
using Domain.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Api.Security;

public class SessionAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly IOptions<RedisOptions> redisOptions;

    public SessionAuthorizeAttribute(
        IOptions<RedisOptions> redisOptions)
    {
        this.redisOptions = redisOptions;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user.Identity?.IsAuthenticated == true)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                context.Result = new UnauthorizedResult();
            }
            else
            {
                var redis = ConnectionMultiplexer.Connect(this.redisOptions.Value.ConnectionString);
                var db = redis.GetDatabase();
                var hasAccess = this.CheckUserAccessInRedis(db, userId);

                if (!hasAccess)
                {
                    // Handle unauthorized access
                    context.Result = new ForbidResult();
                }
            }
        }
        else
        {
            // Handle not authenticated
            context.Result = new UnauthorizedResult();
        }
    }

    private bool CheckUserAccessInRedis(IDatabase db, string userId)
    {
        // Implement your Redis logic here
        // For example, check if a key exists for the user
        return db.KeyExists(userId);
    }
}
