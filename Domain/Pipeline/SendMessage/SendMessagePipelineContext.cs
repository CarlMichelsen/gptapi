using Domain.Dto.Conversation;
using Domain.Entity;
using Domain.Entity.Id;

namespace Domain.Pipeline.SendMessage;

public class SendMessagePipelineContext
{
    public required string ConnectionId { get; init; }
    
    public required UserProfileId UserProfileId { get; set; }

    public required UserMessageData UserMessageData { get; init; }

    public required ConversationAppendData? ConversationAppendData { get; init; }

    public Conversation? Conversation { get; set; }

    public Message? AssistantMessage { get; set; }

    public List<MessageChunkDto> MessageChunkDtos { get; set; } = new();
}
