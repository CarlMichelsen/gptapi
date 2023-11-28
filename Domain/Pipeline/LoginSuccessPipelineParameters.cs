namespace Domain.Pipeline;

public class LoginSuccessPipelineParameters
{
    public required Guid OAuthRecordId { get; init; }

    public required string TokenType { get; init; }

    public required string AccessToken { get; init; }
    
    public string? SteamId { get; set; }

    public bool Authorized { get; set; } = false;

    public string? RedirectUri { get; set; }
}
