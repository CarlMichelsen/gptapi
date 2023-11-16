namespace Domain.Dto.Steam;

public class DevelopmentIdpResponse
{
    public required string SuccessRedirectUrl { get; init; }
    
    public required List<SteamDevelopmentPlayerDto> Players { get; init; }
}
