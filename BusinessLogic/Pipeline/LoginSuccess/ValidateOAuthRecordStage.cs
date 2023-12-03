using Database;
using Domain.Configuration;
using Domain.Entity;
using Domain.Entity.Id;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Factory;
using Interface.Pipeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Pipeline.LoginSuccess;

public class ValidateOAuthRecordStage : IPipelineStage<LoginSuccessPipelineParameters>
{
    private readonly ILogger<ValidateOAuthRecordStage> logger;
    private readonly IOptions<WhitelistOptions> whitelistOptions;
    private readonly ApplicationContext applicationContext;
    private readonly ISteamClientFactory steamClientFactory;

    public ValidateOAuthRecordStage(
        ILogger<ValidateOAuthRecordStage> logger,
        IOptions<WhitelistOptions> whitelistOptions,
        ApplicationContext applicationContext,
        ISteamClientFactory steamClientFactory)
    {
        this.logger = logger;
        this.whitelistOptions = whitelistOptions;
        this.applicationContext = applicationContext;
        this.steamClientFactory = steamClientFactory;
    }

    public async Task<LoginSuccessPipelineParameters> Process(
        LoginSuccessPipelineParameters input,
        CancellationToken cancellationToken)
    {
        var record = await this.applicationContext.OAuthRecord
            .FindAsync(input.OAuthRecordId);
        
        // If this record is not known, don't grant access.
        if (record is null)
        {
            throw new OAuthException("Did not find an OAuthRecord.");
        }

        // If this record was started more than 5 hours ago, don't grant access.
        record.ReturnedFromThirdParty = DateTime.UtcNow;
        if (record.ReturnedFromThirdParty - record.RedirectedToThirdParty > TimeSpan.FromHours(5))
        {
            throw new OAuthException("OAuthRecord is more than 5 hours old.");
        }

        // If this record does not have an accesstoken, don't grant access.
        record.AccessToken = input.AccessToken;
        if (string.IsNullOrWhiteSpace(record.AccessToken))
        {
            throw new OAuthException("No accessToken.");
        }

        // If the accessToken receieved from redirect query parameters can't be exchanged for a steamId, don't grant access.
        var client = this.steamClientFactory.Create();
        var steamId = await client.GetSteamId(record.AccessToken);
        if (steamId is null)
        {
            throw new OAuthException("Failed to exchange accessToken for a steamId.");
        }

        // Save SteamID, verify oAuthRecord and grant access!
        record.UserId = steamId;
        input.SteamId = steamId;
        if (!record.IsCompleted())
        {
            throw new OAuthException("OAuth process should have completed by now.");
        }

        var steamIdWhitelisted = this.whitelistOptions.Value.WhitelistedSteamIds.Exists(w => w == steamId);
        if (!steamIdWhitelisted)
        {
            throw new OAuthException($"Steamid <{steamId}> was not whitelisted and was thus not logged in");
        }

        // Try to get userprofile to assign oAuthRecord to it
        var userProfile = await this.applicationContext.UserProfile
            .FirstOrDefaultAsync(u => 
            u.AuthenticationIdType == Domain.Entity.AuthenticationMethod.Steam
            && u.AuthenticationId == input.SteamId);
        
        // Create a userprofile if none exsists
        if (userProfile is null)
        {
            this.logger.LogWarning("New user logged in <{steamid}>", steamId);
            var now = DateTime.UtcNow;
            userProfile = new UserProfile
            {
                Id = new UserProfileId(Guid.NewGuid()),
                AuthenticationId = steamId,
                AuthenticationIdType = Domain.Entity.AuthenticationMethod.Steam,
                Created = now,
                LastLogin = now,
            };
            this.applicationContext.UserProfile.Add(userProfile);
            this.applicationContext.SaveChanges();
        }

        userProfile.OAuthRecords.Add(record);
        await this.applicationContext.SaveChangesAsync();

        input.UserProfileId = userProfile.Id;
        input.Authorized = true;

        return input;
    }
}
