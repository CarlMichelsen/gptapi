namespace Domain.Configuration;

public class SteamOAuthOptions
{
    public const string SectionName = "SteamOAuth";

    public required string ClientId { get; init; }

    public required string OAuthEndpoint { get; init; }
}
