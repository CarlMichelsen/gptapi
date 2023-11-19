namespace Domain.Dto.Conversation;

public class ConversationMetaDataDto
{
    public required Guid Id { get; init; }

    public required string? Summary { get; set; }

    public required DateTime Created { get; set; }
}
