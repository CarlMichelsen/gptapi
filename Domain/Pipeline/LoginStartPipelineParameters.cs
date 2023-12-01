using Domain.Entity.Id;

namespace Domain.Pipeline;

public class LoginStartPipelineParameters
{
    public required OAuthRecordId OAuthRecordId { get; init; }

    public string? RedirectUri { get; set; }
}
