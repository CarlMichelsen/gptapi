namespace Domain.Pipeline;

public class LoginFailurePipelineParameters
{
    public required Guid OAuthRecordId { get; init; }

    public required string Error { get; init; }

    public string? RedirectUri { get; set; }
}
