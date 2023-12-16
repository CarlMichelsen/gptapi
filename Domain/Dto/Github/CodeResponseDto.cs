using System.Text.Json.Serialization;

namespace Domain.Dto.Github;

public class CodeResponseDto
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("scope")]
    public required string CommaSeparatedScopes { get; init; }

    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }
}
