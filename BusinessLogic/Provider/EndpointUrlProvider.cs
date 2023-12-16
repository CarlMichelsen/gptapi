using Domain;
using Domain.Configuration;
using Domain.Exception;
using Interface.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Provider;

public class EndpointUrlProvider : IEndpointUrlProvider
{
    private readonly LinkGenerator linkGenerator;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IOptions<ApplicationOptions> applicationOptions;

    public EndpointUrlProvider(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor,
        IOptions<ApplicationOptions> applicationOptions)
    {
        this.linkGenerator = linkGenerator;
        this.httpContextAccessor = httpContextAccessor;
        this.applicationOptions = applicationOptions;
    }

    public string GenerateQueryParamsToAppend(Dictionary<string, string> keyValuePairs)
    {
        return "?" + string.Join("&", keyValuePairs.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
    }

    public string GetEndpointUrlFromEndpointName(string endpointName)
    {
        var httpContext = this.httpContextAccessor.HttpContext
            ?? throw new OAuthException("HttpContext not available");

        var redirectUri = this.linkGenerator.GetUriByName(httpContext, endpointName)
            ?? throw new OAuthException("Failed to get development OAuth redirect url");

        return this.applicationOptions.Value.IsDevelopment
            ? redirectUri
            : redirectUri.Replace("http", "https");
    }
}
