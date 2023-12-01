namespace Domain.Entity.Id;

public sealed class MessageId : TypedGuid<MessageId>
{
    public MessageId(Guid value)
        : base(value)
    {
    }
}
