namespace Domain.Entity.Id;

public sealed class ConversationId : TypedGuid<ConversationId>
{
    public ConversationId(Guid value)
        : base(value)
    {
    }
}
