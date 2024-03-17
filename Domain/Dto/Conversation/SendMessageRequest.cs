namespace Domain.Dto.Conversation;

public class SendMessageRequest
{
    public required string MessageContent { get; init; }

    public required Guid? ConversationId { get; init; }

    public required Guid? PreviousMessageId { get; init; }

    public required PromptSettingDto PromptSetting { get; init; }
}
