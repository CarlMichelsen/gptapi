using Database;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Pipeline;

namespace BusinessLogic.Pipeline.LoginFailure;

public class RegisterLoginFailureStage : IPipelineStage<LoginFailurePipelineParameters>
{
    private readonly ApplicationContext applicationContext;

    public RegisterLoginFailureStage(
        ApplicationContext applicationContext)
    {
        this.applicationContext = applicationContext;
    }

    public async Task<LoginFailurePipelineParameters> Process(
        LoginFailurePipelineParameters input,
        CancellationToken cancellationToken)
    {
        var oAuthRecord = await this.applicationContext.OAuthRecord.FindAsync(input.OAuthRecordId)
            ?? throw new PipelineException("Could not find oAuthRecord for LoginFailure");
        
        oAuthRecord.Error = input.Error;
        oAuthRecord.ReturnedFromThirdParty = DateTime.UtcNow;
        await this.applicationContext.SaveChangesAsync();

        return input;
    }
}
