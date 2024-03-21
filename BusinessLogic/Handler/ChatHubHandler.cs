using Domain.Abstractions;
using Domain.Dto;
using Domain.Dto.Conversation;
using Domain.Dto.Session;
using Domain.Entity;
using Domain.Entity.Id;
using Domain.Exception;
using Domain.Pipeline.SendMessage;
using Interface.Handler;
using Interface.Hub;
using Microsoft.AspNetCore.SignalR;

namespace BusinessLogic.Handler;

public class ChatHubHandler : Hub<IChatClient>, IChatServer
{
    protected const string SessionDataItemKey = "SessionDataItemKey";
    private readonly ISendMessagePipelineExecutionHandler sendMessagePipelineExecutionHandler;
    
    protected ChatHubHandler(
        ISendMessagePipelineExecutionHandler sendMessagePipelineExecutionHandler)
    {
        this.sendMessagePipelineExecutionHandler = sendMessagePipelineExecutionHandler;
    }

    public SessionData InitialSessionData => this.Context.Items[SessionDataItemKey] as SessionData
        ?? throw new ServiceException("ChatHubHandler InitialSessionData unavailable");

    public void SendMessage(SendMessageRequest sendMessageRequest)
    {
        var initialContext = this.CreateInitialContext(sendMessageRequest);
        this.sendMessagePipelineExecutionHandler.SendMessage(initialContext, this.HandleError);
    }

    public void CancelMessage(Guid pipelineIdentifier)
    {
        this.sendMessagePipelineExecutionHandler.CancelMessage(pipelineIdentifier);
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
    
    private void HandleError(Error err)
    {
        var errDto = new ErrorDto(err);
        this.Clients.Caller.Error(errDto);
    }

    private SendMessagePipelineContext CreateInitialContext(
        SendMessageRequest sendMessageRequest)
    {
        ConversationAppendData? conversationAppendData = default;
        if (IsDefined(sendMessageRequest?.PreviousMessageId) && IsDefined(sendMessageRequest?.ConversationId))
        {
            conversationAppendData = new ConversationAppendData(
                new MessageId((Guid)sendMessageRequest!.PreviousMessageId!),
                new ConversationId((Guid)sendMessageRequest.ConversationId!));
        }

        return new SendMessagePipelineContext
        {
            LlmProvider = this.MapProvider(sendMessageRequest!.PromptSetting.Provider),
            LlmModel = sendMessageRequest.PromptSetting.Model,
            ConnectionId = this.Context.ConnectionId,
            UserProfileId = this.InitialSessionData.UserProfileId,
            MessageContent = sendMessageRequest!.MessageContent,
            ConversationAppendData = conversationAppendData,
        };
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
