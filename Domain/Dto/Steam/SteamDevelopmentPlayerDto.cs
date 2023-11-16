namespace Domain.Dto.Steam;

public sealed class SteamDevelopmentPlayerDto : SteamPlayerDto
{
    public required string DevelopmentAccessToken { get; init; }
}
