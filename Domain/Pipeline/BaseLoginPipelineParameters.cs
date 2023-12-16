using Domain.Entity;
using Domain.Entity.Id;

namespace Domain.Pipeline;

public abstract class BaseLoginPipelineParameters : ILoginPipelineParameters
{
    public required OAuthRecordId OAuthRecordId { get; init; }

    public required string TokenType { get; init; }

    public required string AccessToken { get; set; }

    public required AuthenticationMethod AuthenticationMethod { get; init; }

    public UserProfileId? UserProfileId { get; set; }
    
    public string? UserId { get; set; }

    public string? RedirectUri { get; set; }
}
