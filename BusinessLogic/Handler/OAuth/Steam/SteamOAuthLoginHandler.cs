using Database;
using Domain.Configuration;
using Domain.Entity;
using Domain.Entity.Id;
using Interface.Handler.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Handler.OAuth.Steam;

public class SteamOAuthLoginHandler : IOAuthLoginHandler
{
    private readonly ILogger<SteamOAuthLoginHandler> logger;
    private readonly ApplicationContext applicationContext;
    private readonly IOptions<SteamOAuthOptions> steamOAuthOptions;

    public SteamOAuthLoginHandler(
        ILogger<SteamOAuthLoginHandler> logger,
        ApplicationContext applicationContext,
        IOptions<SteamOAuthOptions> steamOAuthOptions)
    {
        this.logger = logger;
        this.applicationContext = applicationContext;
        this.steamOAuthOptions = steamOAuthOptions;
    }

    public async Task<IResult> Login()
    {
        try
        {
            var oAuthRecord = await this.RegisterOAuthRecord(new OAuthRecordId(Guid.NewGuid()));
            var url = this.GenerateSteamOAuthUrl(oAuthRecord);
            return Results.Redirect(url);
        }
        catch (Exception e)
        {
            this.logger.LogCritical(
                "Critical failure in {handlername} {exception}",
                nameof(SteamOAuthLoginHandler),
                e);
            
            return Results.StatusCode(500);
        }
    }

    public string GenerateSteamOAuthUrl(OAuthRecord oAuthRecord)
    {
        var parameters = new Dictionary<string, string>
        {
            { "response_type", "token" },
            { "client_id", this.steamOAuthOptions.Value.ClientId },
            { "state", oAuthRecord.Id.ToString() },
        };

        var queryString = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var baseUri = new Uri(this.steamOAuthOptions.Value.OAuthEndpoint);
        return $"{baseUri}?{queryString}";
    }

    private async Task<OAuthRecord> RegisterOAuthRecord(OAuthRecordId id)
    {
        var oAuthRecord = new OAuthRecord
        {
            Id = id,
            AuthenticationMethod = AuthMethods.Steam,
            RedirectedToThirdParty = DateTime.UtcNow,
            ReturnedFromThirdParty = null,
            UserId = null,
            AccessToken = null,
            Error = null,
        };

        this.applicationContext.OAuthRecord.Add(oAuthRecord);
        await this.applicationContext.SaveChangesAsync();

        return oAuthRecord;
    }
}
