using Domain.Entity.Id;

namespace Domain.Pipeline;

public class SteamLoginStartPipelineParameters
{
    public required OAuthRecordId OAuthRecordId { get; init; }

    public string? RedirectUri { get; set; }
}
