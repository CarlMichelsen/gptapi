namespace Domain.Entity.Id;

public class OAuthRecordId : TypedGuid<OAuthRecordId>
{
    public OAuthRecordId(Guid value)
        : base(value)
    {
    }
}
