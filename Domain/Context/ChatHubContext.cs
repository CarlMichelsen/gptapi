namespace Domain.Context;

public class ChatHubContext
{
    public required Guid AuthRecordId { get; init; }

    public required string AccessToken { get; init; }
    
    public required long SteamId { get; init; }
}
