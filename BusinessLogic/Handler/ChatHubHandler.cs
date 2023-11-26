using BusinessLogic.Pipeline.SendMessage;
using Domain.Context;
using Domain.Dto.Conversation;
using Domain.Entity;
using Domain.Exception;
using Domain.Pipeline;
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

    protected ChatHubContext ChatHubContext
    {
        get => (ChatHubContext)this.Context.Items["context"]!;
    }

    public async Task SendMessage(SendMessageRequest sendMessageRequest)
    {
        this.logger.LogInformation(
            "Receieved message from ({steamId}) with content \"{content}\"",
            this.ChatHubContext.SteamId,
            sendMessageRequest.MessageContent);

        var userMessage = new Message
        {
            Id = Guid.Empty,
            Role = Role.User,
            Content = sendMessageRequest.MessageContent,
            Created = DateTime.UtcNow,
            Complete = true,
        };

        var parameter = new SendMessagePipelineParameters
        {
            ConversationId = sendMessageRequest.ConversationId,
            UserId = this.ChatHubContext.SteamId.ToString(),
            ConnectionId = this.Context.ConnectionId,
            UserMessage = userMessage,
        };

        try
        {
            await this.sendMessagePipeline.Execute(
                parameter,
                this.Context.ConnectionAborted);
        }
        catch (PipelineException e)
        {
            this.logger.LogError(
                "A PipelineException happened in the signalR SendMessage method {e}",
                e);
        }
        catch (Exception e)
        {
            this.logger.LogCritical(
                "A critical exception happened in the signalR SendMessage method {e}",
                e);
        }
    }
}
