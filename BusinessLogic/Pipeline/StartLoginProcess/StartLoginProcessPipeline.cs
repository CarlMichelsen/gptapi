using Domain.Pipeline;

namespace BusinessLogic.Pipeline.StartLoginProcess;

public class StartLoginProcessPipeline : Pipeline<StartLoginPipelineParameters>
{
    public StartLoginProcessPipeline(
        ValidateConfigurationStage validateConfigurationStage,
        RegisterLoginAttemptStage registerLoginAttemptStage,
        DeriveRedirectUriStage deriveRedirectUriStage)
    {
        this.AddStage(validateConfigurationStage)
            .AddStage(registerLoginAttemptStage)
            .AddStage(deriveRedirectUriStage);
    }
}
