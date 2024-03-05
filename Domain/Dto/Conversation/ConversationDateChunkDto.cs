namespace Domain.Dto.Conversation;

public record ConversationDateChunkDto(
    string DateText,
    List<ConversationOptionDto> Options);
