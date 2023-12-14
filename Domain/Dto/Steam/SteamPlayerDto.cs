using System.Text.Json.Serialization;
using Domain.OAuth;

namespace Domain.Dto.Steam;

public class SteamPlayerDto : IOAuthUserDataConvertible
{
    [JsonPropertyName("steamid")]
    public required string SteamId { get; set; }

    [JsonPropertyName("communityvisibilitystate")]
    public required int CommunityVisibilityState { get; set; }

    [JsonPropertyName("profilestate")]
    public required int ProfileState { get; set; }

    [JsonPropertyName("personaname")]
    public required string PersonaName { get; set; }

    [JsonPropertyName("lastlogoff")]
    public required long LastLogoff { get; set; }

    [JsonPropertyName("profileurl")]
    public required string ProfileUrl { get; set; }
    
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    [JsonPropertyName("avatarmedium")]
    public string? AvatarMedium { get; set; }

    [JsonPropertyName("avatarfull")]
    public string? AvatarFull { get; set; }

    public OAuthUserData ToOAuthUser()
    {
        return new OAuthUserData
        {
            OAuthId = this.SteamId,
            Name = this.PersonaName,
            AvatarUrl = this.Avatar ?? this.AvatarMedium ?? this.AvatarFull ?? string.Empty,
        };
    }
}
