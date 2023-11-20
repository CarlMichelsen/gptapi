using Domain.Dto.Conversation;

namespace Interface.Hub;

public interface IChatServer
{
    Task SendMessage(SendMessageRequest sendMessageRequest);
}
