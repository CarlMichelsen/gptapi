using Domain.Dto.Conversation;

namespace Interface.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(MessageDto message);
    
    Task ReceiveMessageChunk(MessageChunkDto messageChunk);

    Task Disconnect();
}
