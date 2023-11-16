using System.Text.Json.Serialization;

namespace Domain.Dto.Steam;

public class GetPlayerSummariesResponse
{
    [JsonPropertyName("players")]
    public List<SteamPlayerDto> Players { get; set; } = new List<SteamPlayerDto>();
}
