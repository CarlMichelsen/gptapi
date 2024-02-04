namespace Domain.Dto.Conversation;

public record MessageDto(
    Guid Id,
    string Role,
    string Content,
    DateTime? CompletedUtc,
    DateTime CreatedUtc);