namespace Domain.OAuth;

public class OAuthUserData
{
    public required string OAuthId { get; init; }

    public required string Name { get; set; }

    public required string AvatarUrl { get; set; }
}
