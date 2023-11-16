using BusinessLogic;
using BusinessLogic.Database;
using BusinessLogic.Hub;
using Domain;
using Interface.Client;
using Microsoft.AspNetCore.Authorization;

namespace Api;

[Authorize(AuthenticationSchemes = GptApiConstants.AccessTokenAuthentication)]
public class ChatHub : ChatHubHandler
{
    private readonly ILogger<ChatHub> logger;

    public ChatHub(
        ILogger<ChatHub> logger,
        ILogger<ChatHubHandler> handlerLogger,
        ApplicationContext applicationContext,
        IGptChatClient gptChatClient)
        : base(handlerLogger, applicationContext, gptChatClient)
    {
        this.logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        this.logger.LogInformation("Client\t\"{id}\"\tconnected", this.Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception is null)
        {
            this.logger.LogInformation("Client\t\"{id}\"\tdisconnected", this.Context.ConnectionId);
        }
        else
        {
            this.logger.LogCritical("Client\t\"{id}\"\tdisconnected\t{exception}", this.Context.ConnectionId, exception);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}
