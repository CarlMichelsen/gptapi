using Domain;
using Domain.Configuration;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Pipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Pipeline.StartLoginProcess;

public class DeriveRedirectUriStage : IPipelineStage<StartLoginPipelineParameters>
{
    private readonly IOptions<SteamOAuthOptions> steamOAuthOptions;
    
    private readonly IOptions<ApplicationOptions> applicationOptions;

    private readonly LinkGenerator linkGenerator;

    private readonly IHttpContextAccessor httpContextAccessor;

    public DeriveRedirectUriStage(
        IOptions<SteamOAuthOptions> steamOAuthOptions,
        IOptions<ApplicationOptions> applicationOptions,
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        this.steamOAuthOptions = steamOAuthOptions;
        this.applicationOptions = applicationOptions;
        this.linkGenerator = linkGenerator;
        this.httpContextAccessor = httpContextAccessor;
    }

    public static string ParseQueryParameters(string endpoint, Dictionary<string, string> parameters)
    {
        var queryString = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

        var baseUri = new Uri(endpoint);
        var uri = new Uri(baseUri, $"?{queryString}");

        return $"?{queryString}";
    }

    public Task<StartLoginPipelineParameters> Process(
        StartLoginPipelineParameters input,
        CancellationToken cancellationToken)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "response_type", "token" },
            { "client_id", this.steamOAuthOptions.Value.ClientId },
            { "state", input.OAuthRecordId.ToString() },
        };

        if (this.applicationOptions.Value.IsDevelopment)
        {
            var httpContext = this.httpContextAccessor.HttpContext ?? throw new PipelineException("HttpContext not available");
            var uri = this.linkGenerator.GetUriByName(httpContext, GptApiConstants.DeveloperIdpName, queryParams);
            input.RedirectUri = uri;
            return Task.FromResult(input);
        }

        var prodUri = ParseQueryParameters(this.steamOAuthOptions.Value.OAuthEndpoint, queryParams);
        input.RedirectUri = prodUri;
        return Task.FromResult(input);
    }
}
