using BusinessLogic.Pipeline;
using Domain.Context;
using Domain.Dto.Conversation;
using Domain.Entity;
using Domain.Entity.Id;
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

    protected GptClaims GptClaims
    {
        get => (GptClaims)this.Context.Items["context"]!;
    }

    public async Task SendMessage(SendMessageRequest sendMessageRequest)
    {
        this.logger.LogInformation(
            "Receieved message from ({steamId}) with content \"{content}\"",
            this.GptClaims.AuthenticationId,
            sendMessageRequest.MessageContent);

        var userMessage = new Message
        {
            Id = new MessageId(Guid.NewGuid()),
            Role = Role.User,
            Content = sendMessageRequest.MessageContent,
            Created = DateTime.UtcNow,
            Complete = true,
        };

        var conversationId = sendMessageRequest.ConversationId is null
            ? null
            : new ConversationId((Guid)sendMessageRequest.ConversationId!);

        var parameter = new SendMessagePipelineParameters
        {
            UserProfileId = this.GptClaims.UserProfileId,
            ConversationId = conversationId,
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
