namespace Domain.Entity;

public class UserProfile
{
    public Guid Id { get; init; }

    public required string AuthenticationId { get; init; }

    public required AuthenticationMethod AuthenticationIdType { get; init; }

    public required DateTime Created { get; init; }

    public List<OAuthRecord> OAuthRecords { get; init; } = new();

    public required DateTime LastLogin { get; set; }
}
