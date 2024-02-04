namespace Domain.Dto.Conversation;

public record ConversationDto(
    Guid Id,
    string? Summary,
    List<MessageContainer> Messages,
    DateTime LastAppended);