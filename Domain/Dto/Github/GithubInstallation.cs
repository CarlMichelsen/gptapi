using System.Text.Json.Serialization;

namespace Domain.Dto.Github;

public class GithubInstallation
{
    [JsonPropertyName("permissions")]
    public required GithubAppPermissions Permissions { get; set; }

    [JsonPropertyName("repository_selection")]
    public required string RepositorySelection { get; set; }

    [JsonPropertyName("single_file_name")]
    public string? SingleFileName { get; set; }

    [JsonPropertyName("has_multiple_single_files")]
    public bool HasMultipleSingleFiles { get; set; }

    [JsonPropertyName("single_file_paths")]
    public List<string>? SingleFilePaths { get; set; }

    [JsonPropertyName("repositories_url")]
    public required string RepositoriesUrl { get; set; }

    [JsonPropertyName("account")]
    public required GithubSimpleUser Account { get; set; }
}
