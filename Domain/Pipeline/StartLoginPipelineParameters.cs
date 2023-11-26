namespace Domain.Pipeline;

public class StartLoginPipelineParameters
{
    public required Guid OAuthRecordId { get; init; }

    public string? RedirectUri { get; set; }
}
