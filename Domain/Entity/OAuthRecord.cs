using Domain.Entity.Id;

namespace Domain.Entity;

public class OAuthRecord
{
    public required OAuthRecordId Id { get; init; }

    public required AuthMethods AuthenticationMethod { get; init; }

    public required DateTime RedirectedToThirdParty { get; init; }

    public required DateTime? ReturnedFromThirdParty { get; set; }

    public required string? UserId { get; set; }

    public required string? AccessToken { get; set; }
    
    public required string? Error { get; set; }
}
