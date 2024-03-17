using Domain.Dto.Conversation;
using Domain.Entity;

namespace Domain.Pipeline.SendMessage;

public class SendMessagePipelineContext
{
    public int MaxTokens => 4096;

    public required string LlmModel { get; init; }

    public required LlmProvider LlmProvider { get; init; }

    public required string ConnectionId { get; init; }
    
    public required Guid UserProfileId { get; set; }

    public required string MessageContent { get; init; }

    public required ConversationAppendData? ConversationAppendData { get; init; }

    public int TokenUsage { get; set; }

    public Conversation? Conversation { get; set; }

    public Message? AssistantMessage { get; set; }

    public List<MessageChunkDto> MessageChunkDtos { get; set; } = new();
}
