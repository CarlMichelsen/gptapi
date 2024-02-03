using Domain.Dto.Conversation;

namespace Interface.Hub;

public interface IChatClient
{
    Task ReceiveMessage(MessageDto message);

    Task UpdateMessageId(UpdateMessageIdDto updateMessageId);
    
    Task ReceiveFirstMessage(FirstMessageDto firstMessage);
    
    Task ReceiveMessageChunk(MessageChunkDto messageChunk);

    Task AssignSummaryToConversation(Guid conversationId, string summary);

    Task Disconnect();
}
