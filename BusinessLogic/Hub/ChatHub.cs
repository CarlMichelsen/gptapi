﻿using BusinessLogic.Handler;
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
        this.Context.Items[SessionDataItemKey] = await this.sessionService.GetSessionData()
            ?? throw new ServiceException("ChatHub.OnConnectedAsync session not found");
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