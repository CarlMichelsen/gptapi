namespace Domain.Dto.Session;

public sealed class SessionData
{
    public required Guid SessionDataKey { get; init; }

    public required int AuthenticationMethod { get; init; }

    public required string AuthenticationMethodName { get; init; }

    public required object CodeResponse { get; init; }

    public required UserData User { get; init; }

    public required Guid UserProfileId { get; init; }

    public required DateTime SessionLastUpdatedUtc { get; set; }
}