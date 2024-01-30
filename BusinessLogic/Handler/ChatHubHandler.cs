using BusinessLogic.Pipeline.SendMessage;
using Domain.Dto.Conversation;
using Interface.Factory;
using Interface.Hub;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler;

public class ChatHubHandler : Hub<IChatClient>, IChatServer
{
    private readonly ILogger<ChatHubHandler> logger;
    private readonly IScopedServiceFactory scopedServiceFactory;

    public ChatHubHandler(
        ILogger<ChatHubHandler> logger,
        IScopedServiceFactory scopedServiceFactory)
    {
        this.logger = logger;
        this.scopedServiceFactory = scopedServiceFactory;
    }

    public Task SendMessage(SendMessageRequest sendMessageRequest)
    {
        var sendMessagePipelineResult = this.scopedServiceFactory
            .CreateScopedService<SendMessagePipeline>();
        if (sendMessagePipelineResult.IsError)
        {
            this.logger.LogError("Unable to create SendMessagePipeline");
        }

        var sendMessagePipeline = sendMessagePipelineResult.Unwrap();
        return Task.CompletedTask;
        /*var initialContext = new SendMessagePipelineContext
        {
            ConnectionId = this.Context.ConnectionId,
            
        };

        await sendMessagePipeline.Execute(initialContext, new CancellationToken());*/
    }
}
