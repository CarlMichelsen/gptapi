using BusinessLogic.Handler;
using Interface.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Hub;

[Authorize]
public class ChatHub : ChatHubHandler
{
    private readonly ILogger<ChatHub> logger;

    public ChatHub(
        ILogger<ChatHub> logger,
        ILogger<ChatHubHandler> handlerLogger,
        IScopedServiceFactory scopedServiceFactory)
        : base(handlerLogger, scopedServiceFactory)
    {
        this.logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception is null)
        {
        }
        else
        {
            this.logger.LogCritical(
                "Client\t{id}\tdisconnected\t{exception}",
                this.Context.ConnectionId,
                exception);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}