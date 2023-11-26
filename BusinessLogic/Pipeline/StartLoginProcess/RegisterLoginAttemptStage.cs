using Database;
using Domain.Entity;
using Domain.Pipeline;
using Interface.Pipeline;

namespace BusinessLogic.Pipeline.StartLoginProcess;

public class RegisterLoginAttemptStage : IPipelineStage<StartLoginPipelineParameters>
{
    private readonly ApplicationContext applicationContext;

    public RegisterLoginAttemptStage(
        ApplicationContext applicationContext)
    {
        this.applicationContext = applicationContext;
    }

    public async Task<StartLoginPipelineParameters> Process(
        StartLoginPipelineParameters input,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var oAuthRecord = new OAuthRecord
        {
            Id = input.OAuthRecordId,
            RedirectedToSteam = DateTime.UtcNow,
            ReturnedFromSteam = null,
            SteamId = null,
            AccessToken = null,
            Error = null,
        };

        this.applicationContext.OAuthRecords.Add(oAuthRecord);
        await this.applicationContext.SaveChangesAsync();

        return input;
    }
}