namespace Domain.Pipeline;

public class DiscordLoginSuccessPipelineParameters : BaseLoginPipelineParameters
{
    public required string Code { get; init; }
    
    public required string Scopes { get; init; }
}
