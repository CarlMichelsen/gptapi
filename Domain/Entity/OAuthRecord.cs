namespace Domain.Entity;

public class OAuthRecord
{
    public required Guid Id { get; init; }

    public required AuthenticationMethod ThirdParty { get; init; }

    public required DateTime RedirectedToThirdParty { get; init; }

    public required DateTime? ReturnedFromThirdParty { get; set; }

    public required string? UserId { get; set; }

    public required string? AccessToken { get; set; }
    
    public required string? Error { get; set; }
}
