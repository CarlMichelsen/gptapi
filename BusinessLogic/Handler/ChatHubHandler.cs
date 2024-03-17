using BusinessLogic.Pipeline.SendMessage;
using Domain.Dto;
using Domain.Dto.Conversation;
using Domain.Dto.Session;
using Domain.Entity;
using Domain.Entity.Id;
using Domain.Exception;
using Domain.Pipeline.SendMessage;
using Interface.Factory;
using Interface.Hub;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Handler;

public class ChatHubHandler : Hub<IChatClient>, IChatServer
{
    protected const string SessionDataItemKey = "SessionDataItemKey";
    private readonly ILogger<ChatHubHandler> logger;
    private readonly IScopedServiceFactory scopedServiceFactory;

    protected ChatHubHandler(
        ILogger<ChatHubHandler> logger,
        IScopedServiceFactory scopedServiceFactory)
    {
        this.logger = logger;
        this.scopedServiceFactory = scopedServiceFactory;
    }

    public SessionData InitialSessionData => this.Context.Items[SessionDataItemKey] as SessionData
        ?? throw new ServiceException("ChatHubHandler InitialSessionData unavailable");

    public async Task SendMessage(SendMessageRequest sendMessageRequest)
    {
        var source = new CancellationTokenSource();
        var result = await this.RunSendMessagePipeline(sendMessageRequest, source.Token);
        await this.HandleResult(result);
    }
    
    private static bool IsDefined(Guid? guid)
    {
        if (guid is null)
        {
            return false;
        }

        if (guid == Guid.Empty)
        {
            return false;
        }

        return true;
    }
    
    private async Task HandleResult(Domain.Abstractions.Result<SendMessagePipelineContext> result)
    {
        if (result.IsError)
        {
            var err = new ErrorDto(result.Error!);
            await this.Clients.Caller.Error(err);
        }
    }

    private async Task<Domain.Abstractions.Result<SendMessagePipelineContext>> RunSendMessagePipeline(
        SendMessageRequest sendMessageRequest,
        CancellationToken cancellationToken)
    {
        var sendMessagePipelineResult = this.scopedServiceFactory
            .CreateScopedService<SendMessagePipeline>();
        if (sendMessagePipelineResult.IsError)
        {
            this.logger.LogError("Unable to create SendMessagePipeline");
        }

        ConversationAppendData? conversationAppendData = default;
        if (IsDefined(sendMessageRequest?.PreviousMessageId) && IsDefined(sendMessageRequest?.ConversationId))
        {
            conversationAppendData = new ConversationAppendData(
                new MessageId((Guid)sendMessageRequest!.PreviousMessageId!),
                new ConversationId((Guid)sendMessageRequest.ConversationId!));
        }

        var initialContext = new SendMessagePipelineContext
        {
            LlmProvider = this.MapProvider(sendMessageRequest!.PromptSetting.Provider),
            LlmModel = sendMessageRequest.PromptSetting.Model,
            ConnectionId = this.Context.ConnectionId,
            UserProfileId = this.InitialSessionData.UserProfileId,
            MessageContent = sendMessageRequest!.MessageContent,
            ConversationAppendData = conversationAppendData,
        };

        var sendMessagePipeline = sendMessagePipelineResult.Unwrap();
        return await sendMessagePipeline.Execute(initialContext, cancellationToken);
    }

    private LlmProvider MapProvider(string providerStr)
    {
        return providerStr.ToLower() switch
        {
            "openai" => LlmProvider.OpenAi,
            "anthropic" => LlmProvider.Anthropic,
            _ => throw new LargeLanguageModelException("ProviderMap failed"),
        };
    }
}
