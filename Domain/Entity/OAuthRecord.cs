namespace Domain.Entity;

public class OAuthRecord
{
    public required Guid Id { get; init; }

    public required DateTime RedirectedToSteam { get; init; }

    public required DateTime? ReturnedFromSteam { get; set; }

    public required string? SteamId { get; set; }

    public required string? AccessToken { get; set; }
    
    public required string? Error { get; set; }
}
