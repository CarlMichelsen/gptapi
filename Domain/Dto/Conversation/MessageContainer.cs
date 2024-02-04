namespace Domain.Dto.Conversation;

public class MessageContainer
{
    public required int Index { get; init; }

    public required Dictionary<Guid, MessageDto> MessageOptions { get; init; }

    public required Guid SelectedMessage { get; set; }
}