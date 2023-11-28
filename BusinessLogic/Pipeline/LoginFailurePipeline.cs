using BusinessLogic.Pipeline.LoginFailure;
using Domain.Pipeline;

namespace BusinessLogic.Pipeline;

public class LoginFailurePipeline : Pipeline<LoginFailurePipelineParameters>
{
    public LoginFailurePipeline(
        RegisterLoginFailureStage registerLoginFailureStage)
    {
        this.AddStage(registerLoginFailureStage);
    }
}
