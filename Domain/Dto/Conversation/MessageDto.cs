namespace Domain.Dto.Conversation;

public record MessageDto(
    Guid Id,
    Guid? PreviousMessageId,
    string Role,
    string Content,
    DateTime? CompletedUtc,
    DateTime CreatedUtc,
    UsageDto? UsageDto,
    bool Visible);