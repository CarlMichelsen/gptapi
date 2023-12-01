using Domain.Entity.Id;

namespace Domain.Pipeline;

public class LoginSuccessPipelineParameters
{
    public required OAuthRecordId OAuthRecordId { get; init; }

    public required string TokenType { get; init; }

    public required string AccessToken { get; init; }

    public UserProfileId? UserProfileId { get; set; }
    
    public string? SteamId { get; set; }

    public bool Authorized { get; set; } = false;

    public string? RedirectUri { get; set; }
}
