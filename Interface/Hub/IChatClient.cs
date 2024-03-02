using Domain.Dto.Conversation;

namespace Interface.Hub;

public interface IChatClient
{
    Task ReceiveMessage(ReceiveMessageDto message);
    
    Task ReceiveMessageChunk(MessageChunkDto messageChunk);

    Task AssignSummaryToConversation(Guid conversationId, string summary);

    Task Disconnect();
}
