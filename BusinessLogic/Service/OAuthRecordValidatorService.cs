using Database;
using Domain;
using Domain.Entity;
using Domain.Entity.Id;
using Domain.Exception;
using Domain.Service;
using Interface.Factory;
using Interface.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Service;

public class OAuthRecordValidatorService : IOAuthRecordValidatorService
{
    private readonly ILogger<OAuthRecordValidatorService> logger;
    private readonly ApplicationContext applicationContext;
    private readonly IOAuthClientFactory oAuthClientFactory;

    public OAuthRecordValidatorService(
        ILogger<OAuthRecordValidatorService> logger,
        ApplicationContext applicationContext,
        IOAuthClientFactory oAuthClientFactory)
    {
        this.logger = logger;
        this.applicationContext = applicationContext;
        this.oAuthClientFactory = oAuthClientFactory;
    }

    public async Task<Result<OAuthRecordValidatorResult, string>> ValidateOAuthRecord(
        OAuthRecordId oAuthRecordId,
        string accessToken,
        AuthMethods validAuthenticationMethod)
    {
        var record = await this.applicationContext.OAuthRecord
            .FindAsync(oAuthRecordId);
        
        if (record is null)
        {
            return "Did not find an OAuthRecord.";
        }

        // If this record was started more than 30 minutes ago, don't grant access.
        record.ReturnedFromThirdParty = DateTime.UtcNow;
        if (record.ReturnedFromThirdParty - record.RedirectedToThirdParty > TimeSpan.FromMinutes(30))
        {
            return "OAuthRecord is more than 30 minutes old.";
        }

        // If the response does not have an accesstoken, don't grant access.
        record.AccessToken = accessToken;
        if (string.IsNullOrWhiteSpace(record.AccessToken))
        {
            return "No accessToken.";
        }

        // Only accept specific AuthenticationMethod
        if (record.AuthenticationMethod != validAuthenticationMethod)
        {
            return "AuthenticationMethod invalid.";
        }

        // If the accessToken receieved from redirect query parameters can't be exchanged for a userId, don't grant access.
        string userId;
        try
        {
            var client = this.oAuthClientFactory.Create(record.AuthenticationMethod);
            userId = await client.GetOAuthId(record.AccessToken);
        }
        catch (HttpRequestException)
        {
            return "Did not have access to OAuthId.";
        }
        
        if (userId is null)
        {
            return "Failed to exchange accessToken for a userId.";
        }

        // Save UserId, verify oAuthRecord and grant access!
        record.UserId = userId;
        if (!record.IsCompleted())
        {
            return "OAuth process should be completed by now.";
        }

        // Try to get userprofile to assign oAuthRecord to it
        var userProfile = await this.applicationContext.UserProfile
            .FirstOrDefaultAsync(u => 
            u.AuthenticationIdType == record.AuthenticationMethod && u.AuthenticationId == userId);
        
        // Create a userprofile if none exsists
        if (userProfile is null)
        {
            var authenticationMethodName = Enum.GetName(record.AuthenticationMethod)
                ?? throw new OAuthException("authenticationMethod should be turned into a string here");

            this.logger.LogWarning(
                "New user logged in for the first time through |{authenticationMethodName}| <{userId}>",
                authenticationMethodName,
                userId);
            
            var now = DateTime.UtcNow;
            userProfile = new UserProfile
            {
                Id = new UserProfileId(Guid.NewGuid()),
                AuthenticationId = userId,
                AuthenticationIdType = record.AuthenticationMethod,
                Created = now,
                LastLogin = now,
            };

            this.applicationContext.UserProfile.Add(userProfile);
            this.applicationContext.SaveChanges();
        }

        userProfile.OAuthRecords.Add(record);
        await this.applicationContext.SaveChangesAsync();

        return new OAuthRecordValidatorResult
        {
            UserProfileId = userProfile.Id,
            OAuthRecord = record,
        };
    }
}