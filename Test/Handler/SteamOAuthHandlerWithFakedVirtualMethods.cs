using BusinessLogic.Database;
using BusinessLogic.Handler;
using Domain.Configuration;
using Domain.Entity;
using Interface.Factory;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Test.Handler;

public class SteamOAuthHandlerWithFakedVirtualMethods : SteamOAuthHandler
{
    public SteamOAuthHandlerWithFakedVirtualMethods(
        ILogger<SteamOAuthHandler> logger,
        IHttpContextAccessor httpContextAccessor,
        IOptions<SteamOAuthOptions> steamOAuthOptions,
        IOptions<ApplicationOptions> applicationOptions,
        ISteamClientFactory steamClientFactory,
        ApplicationContext context)
        : base(logger, httpContextAccessor, steamOAuthOptions, applicationOptions, steamClientFactory, context)
    {
    }

    public override Task AddCookieResponseHeader(OAuthRecord record)
    {
        return Task.CompletedTask;
    }

    public override Task SignOut()
    {
        return Task.CompletedTask;
    }
}
