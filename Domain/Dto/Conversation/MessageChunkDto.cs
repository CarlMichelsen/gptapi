namespace Domain.Dto.Conversation;

public record MessageChunkDto(
    int ChunkOrderIndex,
    int Index,
    Guid ConversationId,
    Guid MessageId,
    Guid PreviousMessageId,
    string Role,
    string Content,
    DateTime CreatedUtc);