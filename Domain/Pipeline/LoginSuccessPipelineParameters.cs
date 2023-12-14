using Domain.Entity.Id;

namespace Domain.Pipeline;

public class LoginSuccessPipelineParameters
{
    public required OAuthRecordId OAuthRecordId { get; init; }

    public required string TokenType { get; init; }

    public required string AccessToken { get; init; }

    public UserProfileId? UserProfileId { get; set; }
    
    public string? UserId { get; set; }

    public string? RedirectUri { get; set; }
}
