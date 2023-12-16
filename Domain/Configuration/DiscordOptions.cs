namespace Domain.Configuration;

public class DiscordOptions
{
    public const string SectionName = "Discord";

    public required string WebhookUrl { get; init; }
}
