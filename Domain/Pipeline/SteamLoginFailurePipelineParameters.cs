using Domain.Entity.Id;

namespace Domain.Pipeline;

public class SteamLoginFailurePipelineParameters
{
    public required OAuthRecordId OAuthRecordId { get; init; }

    public required string Error { get; init; }

    public string? RedirectUri { get; set; }
}
