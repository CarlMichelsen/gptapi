namespace Domain.Dto.Conversation;

public class PromptSettingDto
{
    public required string Provider { get; init; }

    public required string Model { get; init; }
}
