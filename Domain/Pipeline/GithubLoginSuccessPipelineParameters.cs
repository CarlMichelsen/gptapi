namespace Domain.Pipeline;

public class GithubLoginSuccessPipelineParameters : BaseLoginPipelineParameters
{
    public required string Code { get; init; }
}
