using System.Text.Json.Serialization;

namespace Domain.Dto.Discord;

public class DiscordWebhookMessage
{
    // Max 2000 characters 
    [JsonPropertyName("content")]
    public required string Content { get; set; }

    // Override the default username of the webhook
    [JsonPropertyName("username")]
    public string? Username { get; set; } 

    // Override the default avatar_url of the webhook
    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }
}
