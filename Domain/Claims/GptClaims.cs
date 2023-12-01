using Domain.Entity.Id;

namespace Domain.Context;

public class GptClaims
{
    public required UserProfileId UserProfileId { get; init; }

    public required OAuthRecordId AuthRecordId { get; init; }

    public required string AuthenticationMethod { get; init; }
    
    public required string AuthenticationId { get; init; }
}
