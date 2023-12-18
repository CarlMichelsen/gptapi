namespace Domain.Configuration;

public class DiscordOptions
{
    public const string SectionName = "Discord";

    public required string WebhookUrl { get; init; }

    public required string ClientId { get; init; }

    public required string ClientSecret { get; init; }
    
    public required string OAuthEndpoint { get; init; }
}
