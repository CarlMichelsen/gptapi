using BusinessLogic.Pipeline.SendMessage;
using Domain.Dto.Conversation;
using Interface.Hub;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler;

public class ChatHubHandler : Hub<IChatClient>, IChatServer
{
    private readonly ILogger<ChatHubHandler> logger;
    private readonly SendMessagePipelineSingleton sendMessagePipeline;

    public ChatHubHandler(
        ILogger<ChatHubHandler> logger,
        SendMessagePipelineSingleton sendMessagePipeline)
    {
        this.logger = logger;
        this.sendMessagePipeline = sendMessagePipeline;
    }

    public async Task SendMessage(SendMessageRequest sendMessageRequest)
    {
    }
}
