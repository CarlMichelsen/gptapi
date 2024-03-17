namespace Domain.Dto.Conversation;

public record MessageChunkDto(
    int ChunkOrderIndex,
    Guid ConversationId,
    Guid MessageId,
    Guid PreviousMessageId,
    string Role,
    string Content,
    DateTime CreatedUtc);