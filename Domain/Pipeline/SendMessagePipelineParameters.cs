using Domain.Entity;
using Domain.Entity.Id;

namespace Domain.Pipeline;

public class SendMessagePipelineParameters
{
    public required UserProfileId UserProfileId { get; init; }

    public required string ConnectionId { get; init; }

    public required Message UserMessage { get; init; }

    public bool StopFurtherMessageStreaming { get; set; } = false;

    public Message? ResponseMessage { get; set; }

    public ConversationId? ConversationId { get; set; }

    public Conversation? Conversation { get; set; } = null;
}
