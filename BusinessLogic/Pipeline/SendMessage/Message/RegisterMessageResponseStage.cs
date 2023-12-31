﻿using BusinessLogic.Hub;
using BusinessLogic.Map;
using Domain.Entity;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Hub;
using Interface.Pipeline;
using Interface.Service;
using Microsoft.AspNetCore.SignalR;

namespace BusinessLogic.Pipeline.SendMessage.Message;

public class RegisterMessageResponseStage : IPipelineStage<SendMessagePipelineParameters>
{
    private readonly IConversationService conversationService;
    private readonly IHubContext<ChatHub, IChatClient> chatHub;

    public RegisterMessageResponseStage(
        IConversationService conversationService,
        IHubContext<ChatHub, IChatClient> chatHub)
    {
        this.conversationService = conversationService;
        this.chatHub = chatHub;
    }

    public async Task<SendMessagePipelineParameters> Process(
        SendMessagePipelineParameters input,
        CancellationToken cancellationToken)
    {
        // I am not checking if the client is still connected...
        var client = this.chatHub.Clients.Client(input.ConnectionId);
        var conv = input.Conversation
            ?? throw new PipelineException("Conversation should be defined at this point");
        
        var responseMessage = input.ResponseMessage
            ?? throw new PipelineException("ResponseMessage should be defined at this point");
        
        // Mark as complete
        responseMessage.Complete = true;
        
        var conversationResult = await this.conversationService
                .AppendConversation(input.UserProfileId, conv.Id, responseMessage);

        input.Conversation = conversationResult.Match(
            (conv) => conv,
            (error) => throw new PipelineException(error));
        
        var msg = input.Conversation.Messages.Last();
        if (msg.Role != Role.Assistant)
        {
            throw new PipelineException(
                $"The last message in the conversation should be from the {Enum.GetName(Role.Assistant)}");
        }

        await client.ReceiveMessage(ConversationMapper.Map(msg));

        return input;
    }
}
