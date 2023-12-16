using Database;
using Domain;
using Domain.Configuration;
using Domain.Entity;
using Domain.Entity.Id;
using Interface.Handler.OAuth;
using Interface.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Handler.OAuth.Development;

public class DevelopmentOAuthLoginHandler : IOAuthLoginHandler
{
    private readonly ILogger<DevelopmentOAuthLoginHandler> logger;
    private readonly ApplicationContext applicationContext;
    private readonly IOptions<ApplicationOptions> applicationOptions;
    private readonly IEndpointUrlProvider endpointUrlProvider;

    public DevelopmentOAuthLoginHandler(
        ILogger<DevelopmentOAuthLoginHandler> logger,
        ApplicationContext applicationContext,
        IOptions<ApplicationOptions> applicationOptions,
        IEndpointUrlProvider endpointUrlProvider)
    {
        this.logger = logger;
        this.applicationContext = applicationContext;
        this.applicationOptions = applicationOptions;
        this.endpointUrlProvider = endpointUrlProvider;
    }

    public async Task<IResult> Login()
    {
        try
        {
            var oAuthRecord = await this.RegisterOAuthRecord(new OAuthRecordId(Guid.NewGuid()));
            var url = this.GenerateDevelopmentOAuthUrl(oAuthRecord);
            return Results.Redirect(url);
        }
        catch (Exception e)
        {
            this.logger.LogCritical(
                "Critical failure in {handlername} {exception}",
                nameof(DevelopmentOAuthLoginHandler),
                e);
            
            return Results.StatusCode(500);
        }
    }

    private string GenerateDevelopmentOAuthUrl(OAuthRecord oAuthRecord)
    {
        var url = this.endpointUrlProvider.GetEndpointUrlFromEndpointName(GptApiConstants.DeveloperIdpName);

        var queryParams = new Dictionary<string, string>
        {
            { "response_type", "token" },
            { "client_id", "development-client-id" },
            { "state", oAuthRecord.Id.ToString() },
        };

        return url + this.endpointUrlProvider.GenerateQueryParamsToAppend(queryParams);
    }

    private async Task<OAuthRecord> RegisterOAuthRecord(OAuthRecordId id)
    {
        var oAuthRecord = new OAuthRecord
        {
            Id = id,
            AuthenticationMethod = AuthenticationMethod.Development,
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
