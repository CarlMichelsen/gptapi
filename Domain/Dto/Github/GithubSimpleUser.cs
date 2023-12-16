using System.Text.Json.Serialization;
using Domain.OAuth;

namespace Domain.Dto.Github;

public class GithubSimpleUser : GithubSimpleUserUrl, IOAuthUserDataConvertible
{
    [JsonPropertyName("login")]
    public required string Login { get; set; }

    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("node_id")]
    public required string NodeId { get; set; }

    [JsonPropertyName("avatar_url")]
    public required string AvatarUrl { get; set; }

    [JsonPropertyName("gravatar_id")]
    public required string GravatarId { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("site_admin")]
    public required bool SiteAdmin { get; set; }

    [JsonPropertyName("starred_at")]
    public required string StarredAt { get; set; }

    public OAuthUserData ToOAuthUser()
    {
        return new OAuthUserData
        {
            OAuthId = this.Id.ToString(),
            Name = this.Login,
            AvatarUrl = this.AvatarUrl,
        };
    }
}