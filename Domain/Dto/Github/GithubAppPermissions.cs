using System.Text.Json.Serialization;

namespace Domain.Github;

public class GithubAppPermissions
{
    [JsonPropertyName("actions")]
    public required string Actions { get; set; }

    [JsonPropertyName("administration")]
    public string? Administration { get; set; }
}
