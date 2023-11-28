using Database;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Factory;
using Interface.Pipeline;

namespace BusinessLogic.Pipeline.LoginSuccess;

public class ValidateOAuthRecordStage : IPipelineStage<LoginSuccessPipelineParameters>
{
    private readonly ApplicationContext applicationContext;
    private readonly ISteamClientFactory steamClientFactory;

    public ValidateOAuthRecordStage(
        ApplicationContext applicationContext,
        ISteamClientFactory steamClientFactory)
    {
        this.applicationContext = applicationContext;
        this.steamClientFactory = steamClientFactory;
    }

    public async Task<LoginSuccessPipelineParameters> Process(
        LoginSuccessPipelineParameters input,
        CancellationToken cancellationToken)
    {
        var record = await this.applicationContext.OAuthRecords
            .FindAsync(input.OAuthRecordId);
        
        // If this record is not known, don't grant access.
        if (record is null)
        {
            return input;
        }

        // If this record was started more than 5 hours ago, don't grant access.
        record.ReturnedFromSteam = DateTime.UtcNow;
        if (record.ReturnedFromSteam - record.RedirectedToSteam > TimeSpan.FromHours(5))
        {
            return input;
        }

        // If this record does not have an accesstoken, don't grant access.
        record.AccessToken = input.AccessToken;
        if (string.IsNullOrWhiteSpace(record.AccessToken))
        {
            return input;
        }

        // If the accessToken receieved from redirect query parameters can't be exchanged for a steamId, don't grant access.
        var client = this.steamClientFactory.Create();
        var steamId = await client.GetSteamId(record.AccessToken);
        if (string.IsNullOrWhiteSpace(steamId))
        {
            return input;
        }

        // Save SteamID, verify oAuthRecord and grant access!
        record.SteamId = steamId;
        input.SteamId = record.SteamId;
        if (!record.IsCompleted())
        {
            throw new OAuthException("OAuth process should have completed by now.");
        }

        await this.applicationContext.SaveChangesAsync();
        input.Authorized = true;
        return input;
    }
}
