﻿using Database;
using Domain.Entity;
using Domain.Exception;
using Domain.Pipeline;
using Domain.Service;
using Interface.Factory;
using Interface.Pipeline;
using Interface.Service;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Pipeline.Steam.LoginSuccess;

public class ValidateSteamOAuthRecordStage : IPipelineStage<ILoginPipelineParameters>
{
    private readonly ILogger<ValidateSteamOAuthRecordStage> logger;
    private readonly IOAuthRecordValidatorService oAuthRecordValidatorService;
    private readonly ApplicationContext applicationContext;
    private readonly IOAuthClientFactory oAuthClientFactory;

    public ValidateSteamOAuthRecordStage(
        ILogger<ValidateSteamOAuthRecordStage> logger,
        IOAuthRecordValidatorService oAuthRecordValidatorService,
        ApplicationContext applicationContext,
        IOAuthClientFactory oAuthClientFactory)
    {
        this.logger = logger;
        this.oAuthRecordValidatorService = oAuthRecordValidatorService;
        this.applicationContext = applicationContext;
        this.oAuthClientFactory = oAuthClientFactory;
    }

    public async Task<ILoginPipelineParameters> Process(
        ILoginPipelineParameters input,
        CancellationToken cancellationToken)
    {
        var oAuthRecordValidationResult = await this.oAuthRecordValidatorService.ValidateOAuthRecord(
            input.OAuthRecordId,
            input.AccessToken,
            AuthMethods.Steam);
        
        return oAuthRecordValidationResult.Match(
            (oAuthRecord) => this.HandlePipelineParameters(input, oAuthRecord),
            (error) => throw new OAuthException(error));
    }

    private ILoginPipelineParameters HandlePipelineParameters(
        ILoginPipelineParameters input,
        OAuthRecordValidatorResult oAuthRecordValidatorResult)
    {
        input.UserId = oAuthRecordValidatorResult.OAuthRecord.UserId;
        input.UserProfileId = oAuthRecordValidatorResult.UserProfileId;
        return input;
    }

    /*public async Task<LoginSuccessPipelineParameters> Process(
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

        // If the response does not have an accesstoken, don't grant access.
        record.AccessToken = input.AccessToken;
        if (string.IsNullOrWhiteSpace(record.AccessToken))
        {
            throw new OAuthException("No accessToken.");
        }

        // Only accept Steam or Development AuthenticationMethod
        var method = record.AuthenticationMethod;
        if (!method.HasFlag(AuthenticationMethod.Steam) && !method.HasFlag(AuthenticationMethod.Development))
        {
            throw new OAuthException("AuthenticationMethod invalid.");
        }

        // If the accessToken receieved from redirect query parameters can't be exchanged for a steamId, don't grant access.
        var client = this.oAuthClientFactory.Create(record.AuthenticationMethod);
        var steamId = await client.GetOAuthId(record.AccessToken);
        if (steamId is null)
        {
            throw new OAuthException("Failed to exchange accessToken for a steamId.");
        }

        // Save SteamID, verify oAuthRecord and grant access!
        record.UserId = steamId;
        input.UserId = steamId;
        if (!record.IsCompleted())
        {
            throw new OAuthException("OAuth process should have completed by now.");
        }

        var steamIdWhitelisted = this.whitelistOptions.Value.WhitelistedUserIds.Exists(w => w == steamId);
        if (!steamIdWhitelisted)
        {
            throw new OAuthException($"Steamid <{steamId}> was not whitelisted and was thus not logged in");
        }

        // Try to get userprofile to assign oAuthRecord to it
        var userProfile = await this.applicationContext.UserProfile
            .FirstOrDefaultAsync(u => 
            u.AuthenticationIdType == Domain.Entity.AuthenticationMethod.Steam
            && u.AuthenticationId == input.UserId);
        
        // Create a userprofile if none exsists
        if (userProfile is null)
        {
            this.logger.LogWarning("New user logged in <{steamid}>", steamId);
            var now = DateTime.UtcNow;
            userProfile = new UserProfile
            {
                Id = new UserProfileId(Guid.NewGuid()),
                AuthenticationId = steamId,
                AuthenticationIdType = AuthenticationMethod.Steam,
                Created = now,
                LastLogin = now,
            };
            this.applicationContext.UserProfile.Add(userProfile);
            this.applicationContext.SaveChanges();
        }

        userProfile.OAuthRecords.Add(record);
        await this.applicationContext.SaveChangesAsync();

        input.UserProfileId = userProfile.Id;
        return input;
    }*/
}
