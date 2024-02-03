namespace Domain.Dto.Conversation;

public record ConversationMetaDataDto(
    Guid Id,
    string? Summary,
    DateTime LastAppendedUtc,
    DateTime CreatedUtc);