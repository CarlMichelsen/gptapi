using BusinessLogic.Pipeline.Steam.LoginFailure;
using Domain.Pipeline;

namespace BusinessLogic.Pipeline.Steam;

public class SteamLoginFailurePipeline : Pipeline<LoginFailurePipelineParameters>
{
    public SteamLoginFailurePipeline(
        RegisterLoginFailureStage registerLoginFailureStage)
    {
        this.AddStage(registerLoginFailureStage);
    }
}
