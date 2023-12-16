using Domain.Pipeline;
using Interface.Pipeline;

namespace BusinessLogic;

public class ValidateGithubOAuthRecordStage : IPipelineStage<ILoginPipelineParameters>
{
    public Task<ILoginPipelineParameters> Process(ILoginPipelineParameters input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
