using System.Security.Claims;
using BusinessLogic.Handler;
using BusinessLogic.Pipeline;
using Domain.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Hub;

[Authorize]
public class ChatHub : ChatHubHandler
{
    private readonly ILogger<ChatHub> logger;

    public ChatHub(
        ILogger<ChatHub> logger,
        ILogger<ChatHubHandler> handlerLogger,
        SendMessagePipeline sendMessagePipeline)
        : base(handlerLogger, sendMessagePipeline)
    {
        this.logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var httpContext = this.Context.GetHttpContext();
            var claims = httpContext!.User.Claims.ToList();
            var context = new ChatHubContext
            {
                AuthRecordId = Guid.Parse(claims.First(c => c.Type == ClaimTypes.Name).Value),
                AccessToken = claims.First(c => c.Type == "AccessToken").Value,
                SteamId = long.Parse(claims.First(c => c.Type == "SteamId").Value),
            };

            this.Context.Items.Add("context", context);
        }
        catch (Exception e)
        {
            this.logger.LogCritical("Failed to read data from claims, terminating connection. {e}", e);
            this.Context.Abort();
            return;
        }

        this.logger.LogInformation("Client\t{id}\tconnected ({userId})", this.Context.ConnectionId, this.ChatHubContext.SteamId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception is null)
        {
            this.logger.LogInformation("Client\t{id}\tdisconnected ({userId})", this.Context.ConnectionId, this.ChatHubContext.SteamId);
        }
        else
        {
            this.logger.LogCritical("Client\t{id}\tdisconnected\t{exception}", this.Context.ConnectionId, exception);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}