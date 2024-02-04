using Domain.Dto.Conversation;
using Domain.Entity;

namespace Domain.Pipeline.SendMessage;

public class SendMessagePipelineContext
{
    public required string ConnectionId { get; init; }
    
    public required Guid UserProfileId { get; set; }

    public required UserMessageData UserMessageData { get; init; }

    public required ConversationAppendData? ConversationAppendData { get; init; }

    public Conversation? Conversation { get; set; }

    public Message? UserMessage { get; set; }

    public Message? AssistantMessage { get; set; }

    public List<MessageChunkDto> MessageChunkDtos { get; set; } = new();
}
