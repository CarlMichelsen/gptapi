using Domain.Dto.Conversation;
using Domain.Entity;

namespace Domain.Pipeline.SendMessage;

public class SendMessagePipelineContext
{
    public string LlmModel => "gpt-4";

    public int MaxTokens => 6000;

    public LlmProvider LlmProvider => LlmProvider.OpenAi;

    public required string ConnectionId { get; init; }
    
    public required Guid UserProfileId { get; set; }

    public required string MessageContent { get; init; }

    public required ConversationAppendData? ConversationAppendData { get; init; }

    public int TokenUsage { get; set; }

    public Conversation? Conversation { get; set; }

    public Message? AssistantMessage { get; set; }

    public List<MessageChunkDto> MessageChunkDtos { get; set; } = new();
}
