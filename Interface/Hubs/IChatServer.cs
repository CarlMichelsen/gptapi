using Domain.Dto.Conversation;

namespace Interface.Hubs;

public interface IChatServer
{
    Task SendMessage(SendMessageRequest sendMessageRequest);
}
