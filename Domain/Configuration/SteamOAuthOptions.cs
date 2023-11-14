namespace Domain.Configuration;

public class SteamOAuthOptions
{
    public const string SectionName = "SteamOAuth";

    public required string ClientId { get; init; }

    /// <summary>
    /// Gets endpoint that user will be redirected to in order to log in.
    /// If this configuration value is not defined, the user should be redirected to developer IDP.
    /// </summary>
    /// <value>Nullable string</value>
    public required string? OAuthEndpoint { get; init; }
}
