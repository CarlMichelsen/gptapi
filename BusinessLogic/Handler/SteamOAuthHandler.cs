using BusinessLogic.Database;
using Domain;
using Domain.Configuration;
using Interface.Handler;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Handler;

public class SteamOAuthHandler : ISteamOAuthHandler
{
    private readonly ILogger<SteamOAuthHandler> logger;
    private readonly IOptions<SteamOAuthOptions> steamOAuthOptions;
    private readonly ApplicationContext context;

    public SteamOAuthHandler(
        ILogger<SteamOAuthHandler> logger,
        IOptions<SteamOAuthOptions> steamOAuthOptions,
        ApplicationContext context)
    {
        this.logger = logger;
        this.steamOAuthOptions = steamOAuthOptions;
        this.context = context;
    }

    public async Task<IResult> SteamLogin()
    {
        var record = new OAuthRecord
        {
            Id = Guid.NewGuid(),
            RedirectedToSteam = DateTime.UtcNow,
            ReturnedFromSteam = null,
            ClientId = null,
            AccessToken = null,
            Error = null,
        };
        var uri = this.SteamOAuthRedirectUri(record.Id);
        await this.context.OAuthRecords.AddAsync(record);
        this.logger.LogInformation("{recordId}: User is being redirected to steam for oAuth login", record.Id);

        return Results.Redirect(uri.AbsoluteUri);
    }

    public async Task<IResult> SteamLoginSuccess(
        Guid oAuthRecordId,
        string accessToken)
    {
        var record = await this.context.OAuthRecords
            .FindAsync(oAuthRecordId);
        if (record is null)
        {
            return Results.NotFound();
        }

        record.AccessToken = accessToken;
        record.ReturnedFromSteam = DateTime.UtcNow;
        await this.context.SaveChangesAsync();
        this.logger.LogInformation("{recordId}: User successfully completed oAuth login", record.Id);

        return Results.Accepted($"Login success {record.AccessToken}");
    }

    public async Task<IResult> SteamLoginFailure(
        Guid oAuthRecordId,
        string error)
    {
        var record = await this.context.OAuthRecords
            .FindAsync(oAuthRecordId);
        if (record is null)
        {
            return Results.NotFound();
        }

        record.Error = error;
        record.ReturnedFromSteam = DateTime.UtcNow;
        await this.context.SaveChangesAsync();
        this.logger.LogInformation("{recordId}: User failed oAuth login", record.Id);

        return Results.Unauthorized();
    }

    private Uri SteamOAuthRedirectUri(Guid recordId)
    {
        var endpoint = this.steamOAuthOptions.Value.OAuthEndpoint ?? "/";
        var clientId = this.steamOAuthOptions.Value.ClientId;
        return new Uri($"${endpoint}?response_type=token&client_id={clientId}&state={recordId}");
    }
}
