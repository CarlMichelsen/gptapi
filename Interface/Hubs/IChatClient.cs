using Domain.Dto;

namespace Interface.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(MessageDto message);
    Task ReceiveMessageChunk(MessageChunkDto messageChunk);

    Task Disconnect();
}
