namespace Domain.Dto.Conversation;

public record ConversationOptionDto(
    Guid Id,
    string? Summary,
    DateTime LastAppendedUtc,
    DateTime CreatedUtc);