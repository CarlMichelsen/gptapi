using BusinessLogic.Pipeline.Steam.LoginFailure;
using Domain.Pipeline;

namespace BusinessLogic.Pipeline.Steam;

public class SteamLoginFailurePipeline : Pipeline<SteamLoginFailurePipelineParameters>
{
    public SteamLoginFailurePipeline(
        RegisterLoginFailureStage registerLoginFailureStage)
    {
        this.AddStage(registerLoginFailureStage);
    }
}
