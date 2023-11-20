using Domain.Entity;

namespace Domain.Pipeline;

public class SendMessagePipelineParameter
{
    public required string UserId { get; init; }

    public required string ConnectionId { get; init; }

    public required Message UserMessage { get; init; }

    public Message? ResponseMessage { get; set; }

    public Guid? ConversationId { get; set; }

    public Conversation? Conversation { get; set; } = null;
}
