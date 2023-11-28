namespace Domain.Pipeline;

public class LoginStartPipelineParameters
{
    public required Guid OAuthRecordId { get; init; }

    public string? RedirectUri { get; set; }
}
