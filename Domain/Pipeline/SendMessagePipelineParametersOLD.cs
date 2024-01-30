using Domain.Entity;
using Domain.Entity.Id;

namespace Domain.Pipeline;

[Obsolete("Not used anymore", true)]
public class SendMessagePipelineParametersOLD
{
    public required UserProfileId UserProfileId { get; init; }

    public required string ConnectionId { get; init; }

    public required Message UserMessage { get; init; }

    public Message? ResponseMessage { get; set; }

    public ConversationId? ConversationId { get; set; }

    public Conversation? Conversation { get; set; } = null;
}
