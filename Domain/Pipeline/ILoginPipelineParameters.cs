using Domain.Entity;
using Domain.Entity.Id;

namespace Domain.Pipeline;

public interface ILoginPipelineParameters
{
    OAuthRecordId OAuthRecordId { get; init; }

    string TokenType { get; init; }

    string AccessToken { get; init; }

    AuthenticationMethod AuthenticationMethod { get; init; }

    UserProfileId? UserProfileId { get; set; }
    
    string? UserId { get; set; }

    string? RedirectUri { get; set; }
}