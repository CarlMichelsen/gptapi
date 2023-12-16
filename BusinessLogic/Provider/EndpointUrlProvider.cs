using Domain;
using Domain.Exception;
using Interface.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BusinessLogic.Provider;

public class EndpointUrlProvider : IEndpointUrlProvider
{
    private readonly LinkGenerator linkGenerator;
    private readonly IHttpContextAccessor httpContextAccessor;

    public EndpointUrlProvider(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        this.linkGenerator = linkGenerator;
        this.httpContextAccessor = httpContextAccessor;
    }

    public string GetEndpointUrlFromEndpointName(string endpointName)
    {
        var httpContext = this.httpContextAccessor.HttpContext
            ?? throw new OAuthException("HttpContext not available");

        var redirectUri = this.linkGenerator.GetUriByName(httpContext, endpointName)
            ?? throw new OAuthException("Failed to get development OAuth redirect url");

        return redirectUri.Replace("http", "https");
    }
}
