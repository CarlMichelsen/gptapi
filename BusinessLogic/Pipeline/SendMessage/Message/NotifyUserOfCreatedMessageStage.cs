using BusinessLogic.Hub;
using BusinessLogic.Map;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Hub;
using Interface.Pipeline;
using Microsoft.AspNetCore.SignalR;

namespace BusinessLogic.Pipeline.SendMessage.Message;

public class NotifyUserOfCreatedMessageStage : IPipelineStage<SendMessagePipelineParameters>
{
    private readonly IHubContext<ChatHub, IChatClient> chatHub;

    public NotifyUserOfCreatedMessageStage(
        IHubContext<ChatHub, IChatClient> chatHub)
    {
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
        var lastMessage = conv.Messages.Last();

        var notifyTask = input.ConversationId is null
            ? client.ReceiveFirstMessage(ConversationMapper.MapFirstMessage(conv.Id, lastMessage))
            : client.ReceiveMessage(ConversationMapper.Map(lastMessage));

        await notifyTask;
        
        return input;
    }
}
