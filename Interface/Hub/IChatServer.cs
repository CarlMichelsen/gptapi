using Domain.Dto.Conversation;

namespace Interface.Hub;

public interface IChatServer
{
    void SendMessage(SendMessageRequest sendMessageRequest);

    void CancelMessage(Guid pipelineIdentifier);
}
