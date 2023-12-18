using System.Text.Json.Serialization;
using Domain.OAuth;

namespace Domain.Dto.Discord;

public class DiscordUser : IOAuthUserDataConvertible
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("username")]
    public required string Username { get; init; }

    [JsonPropertyName("avatar")]
    public required string Avatar { get; init; }

    [JsonPropertyName("discriminator")]
    public required string Discriminator { get; init; }

    [JsonPropertyName("public_flags")]
    public required int PublicFlags { get; init; }

    [JsonPropertyName("premium_type")]
    public required int PremiumType { get; init; }

    [JsonPropertyName("flags")]
    public required int Flags { get; init; }

    [JsonPropertyName("banner")]
    public string? Banner { get; init; }

    [JsonPropertyName("banner_color")]
    public string? BannerColor { get; init; }

    [JsonPropertyName("accent_color")]
    public required long AccentColor { get; init; }

    [JsonPropertyName("global_name")]
    public required string GlobalName { get; init; }

    [JsonPropertyName("avatar_decoration_data")]
    public object? AvatarDecorationData { get; init; }

    [JsonPropertyName("mfa_enabled")]
    public required bool MultiFactorAuthenticationEnabled { get; init; }

    [JsonPropertyName("locale")]
    public required string Locale { get; init; }

    [JsonPropertyName("email")]
    public required string Email { get; init; }

    [JsonPropertyName("verified")]
    public required bool Verified { get; init; }

    public string DiscordCdnBaseUrl { get => "https://cdn.discordapp.com"; }

    public OAuthUserData ToOAuthUser()
    {
        return new OAuthUserData
        {
            OAuthId = this.Id,
            Name = this.GlobalName,
            AvatarUrl = $"{this.DiscordCdnBaseUrl}/avatars/{this.Id}/{this.Avatar}",
        };
    }
}
