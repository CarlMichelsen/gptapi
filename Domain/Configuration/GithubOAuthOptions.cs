namespace Domain.Configuration;

public class GithubOAuthOptions
{
    public const string SectionName = "GithubOAuth";

    public required string ClientId { get; init; }

    public required string ClientSecret { get; init; }

    public required string OAuthEndpoint { get; init; }
}
