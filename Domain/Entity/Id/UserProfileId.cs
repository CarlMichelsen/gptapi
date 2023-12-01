namespace Domain.Entity.Id;

public sealed class UserProfileId : TypedGuid<UserProfileId>
{
    public UserProfileId(Guid value)
        : base(value)
    {
    }
}
