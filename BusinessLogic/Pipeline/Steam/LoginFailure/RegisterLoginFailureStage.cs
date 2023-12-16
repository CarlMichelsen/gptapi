using Database;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Pipeline;

namespace BusinessLogic.Pipeline.Steam.LoginFailure;

public class RegisterLoginFailureStage : IPipelineStage<SteamLoginFailurePipelineParameters>
{
    private readonly ApplicationContext applicationContext;

    public RegisterLoginFailureStage(
        ApplicationContext applicationContext)
    {
        this.applicationContext = applicationContext;
    }

    public async Task<SteamLoginFailurePipelineParameters> Process(
        SteamLoginFailurePipelineParameters input,
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
