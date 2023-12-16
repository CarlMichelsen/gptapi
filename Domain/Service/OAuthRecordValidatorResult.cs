using Domain.Entity;
using Domain.Entity.Id;

namespace Domain.Service;

public struct OAuthRecordValidatorResult
{
    public required UserProfileId UserProfileId { get; init; }
    
    public required OAuthRecord OAuthRecord { get; init; }
}
