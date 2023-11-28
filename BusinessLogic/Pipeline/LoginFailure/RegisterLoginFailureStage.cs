using Domain.Pipeline;
using Interface.Pipeline;

namespace BusinessLogic.Pipeline.LoginFailure;

public class RegisterLoginFailureStage : IPipelineStage<LoginFailurePipelineParameters>
{
    public Task<LoginFailurePipelineParameters> Process(
        LoginFailurePipelineParameters input,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
