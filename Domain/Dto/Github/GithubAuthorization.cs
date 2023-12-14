using System.Text.Json.Serialization;

namespace Domain.Github;

public class GithubAuthorization
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("scopes")]
    public List<string>? Scopes { get; set; }

    [JsonPropertyName("token")]
    public required string Token { get; set; }

    [JsonPropertyName("token_last_eight")]
    public string? TokenLastEight { get; set; }

    [JsonPropertyName("hashed_token")]
    public string? HashedToken { get; set; }

    [JsonPropertyName("app")]
    public required GithubApp App { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonPropertyName("note_url")]
    public string? NoteUrl { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("fingerprint")]
    public string? Fingerprint { get; set; }

    [JsonPropertyName("user")]
    public required GithubUser User { get; set; }

    [JsonPropertyName("installation")]
    public required GithubInstallation Installation { get; set; }

    [JsonPropertyName("expires_at")]
    public DateTime? ExpiresAt { get; set; }
}
