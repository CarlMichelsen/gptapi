using Domain;
using Domain.Configuration;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Pipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Pipeline.LoginSuccess;

public class DeriveSuccessRedirectUriStage : IPipelineStage<LoginSuccessPipelineParameters>
{
    private readonly IOptions<ApplicationOptions> applicationOptions;
    private readonly LinkGenerator linkGenerator;
    private readonly IHttpContextAccessor httpContextAccessor;

    public DeriveSuccessRedirectUriStage(
        IOptions<ApplicationOptions> applicationOptions,
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        this.applicationOptions = applicationOptions;
        this.linkGenerator = linkGenerator;
        this.httpContextAccessor = httpContextAccessor;
    }

    public Task<LoginSuccessPipelineParameters> Process(
        LoginSuccessPipelineParameters input,
        CancellationToken cancellationToken)
    {
        if (!input.Authorized)
        {
            return Task.FromResult(input);
        }

        if (this.applicationOptions.Value.IsDevelopment)
        {
            input.RedirectUri = GptApiConstants.DeveloperFrontendUrl;
            return Task.FromResult(input);
        }

        var httpContext = this.httpContextAccessor.HttpContext
            ?? throw new PipelineException("HttpContext not available during DeriveSuccessRedirectUriStage pipeline stage.");
        var uri = this.linkGenerator.GetUriByName(httpContext, GptApiConstants.FrontendEndpointName);

        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new PipelineException("LinkGenerator could not find frontendUrl by endpoint name.");
        }

        input.RedirectUri = uri;
        return Task.FromResult(input);
    }
}
