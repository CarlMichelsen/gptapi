using BusinessLogic.Pipeline.LoginStart;
using Domain.Pipeline;

namespace BusinessLogic.Pipeline;

public class LoginStartPipeline : Pipeline<LoginStartPipelineParameters>
{
    public LoginStartPipeline(
        RegisterLoginAttemptStage registerLoginAttemptStage,
        DeriveRedirectUriStage deriveRedirectUriStage)
    {
        this.AddStage(registerLoginAttemptStage)
            .AddStage(deriveRedirectUriStage);
    }
}
