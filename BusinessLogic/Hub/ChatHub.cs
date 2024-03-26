using BusinessLogic.Handler;
using Domain.Exception;
using Interface.Handler;
using Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Hub;

[Authorize]
public class ChatHub : ChatHubHandler
{
    private readonly ILogger<ChatHub> logger;
    private readonly ISessionService sessionService;

    public ChatHub(
        ILogger<ChatHub> logger,
        ISessionService sessionService,
        ISendMessagePipelineExecutionHandler sendMessagePipelineExecutionHandler)
        : base(sendMessagePipelineExecutionHandler)
    {
        this.logger = logger;
        this.sessionService = sessionService;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        var sessionResult = await this.sessionService.GetSessionData();

        if (sessionResult.IsError)
        {
            this.Context.Abort();
            this.logger.LogCritical(
                "Connection aborted because of missing sessiondata after authorization {errorCode}: {errorDescription}",
                sessionResult.Error!.Code,
                sessionResult.Error!.Description);
            
            return;
        }

        this.Context.Items[SessionDataItemKey] = sessionResult.Unwrap();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception is null)
        {
            this.logger.LogInformation(
                "Client\t{id}\tdisconnected safely",
                this.Context.ConnectionId);
        }
        else
        {
            this.logger.LogCritical(
                "Client\t{id}\tdisconnected unexpectedly\t{exception}",
                this.Context.ConnectionId,
                exception);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}