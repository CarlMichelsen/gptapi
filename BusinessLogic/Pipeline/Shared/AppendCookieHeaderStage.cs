using System.Security.Claims;
using Domain.Claims;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Pipeline;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Pipeline.Shared;

public class AppendCookieHeaderStage : IPipelineStage<ILoginPipelineParameters>
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public AppendCookieHeaderStage(
        IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<ILoginPipelineParameters> Process(
        ILoginPipelineParameters input,
        CancellationToken cancellationToken)
    {
        var httpContext = this.httpContextAccessor.HttpContext
            ?? throw new PipelineException("HttpContext not available during AppendCookieHeaderStage pipeline stage.");
        
        await this.AddCookieResponseHeader(httpContext, input);
        
        return input;
    }

    public virtual async Task AddCookieResponseHeader(
        HttpContext httpContext,
        ILoginPipelineParameters parameters)
    {
        var authenticationMethodName = Enum.GetName(parameters.AuthenticationMethod)
            ?? throw new PipelineException("authenticationMethod should be turned into a string here");

        var userId = parameters.UserId
            ?? throw new PipelineException("AuthenticationId should exsist here");

        var claims = new List<Claim>
        {
            new Claim(GptClaimKeys.UserProfileId, parameters.UserProfileId!.ToString()),
            new Claim(GptClaimKeys.OAuthRecordId, parameters.OAuthRecordId.ToString()),
            new Claim(GptClaimKeys.AuthenticationMethod, authenticationMethodName),
            new Claim(GptClaimKeys.AuthenticationId, userId),
        };
        var claimsIdentity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties { };

        await this.httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }
}