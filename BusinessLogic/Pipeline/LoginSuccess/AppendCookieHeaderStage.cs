using System.Security.Claims;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Pipeline;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Pipeline.LoginSuccess;

public class AppendCookieHeaderStage : IPipelineStage<LoginSuccessPipelineParameters>
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public AppendCookieHeaderStage(
        IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<LoginSuccessPipelineParameters> Process(
        LoginSuccessPipelineParameters input,
        CancellationToken cancellationToken)
    {
        if (!input.Authorized)
        {
            return input;
        }

        var httpContext = this.httpContextAccessor.HttpContext
            ?? throw new PipelineException("HttpContext not available during AppendCookieHeaderStage pipeline stage.");
        
        await this.AddCookieResponseHeader(httpContext, input);
        
        return input;
    }

    public virtual async Task AddCookieResponseHeader(
        HttpContext httpContext,
        LoginSuccessPipelineParameters parameters)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, parameters.OAuthRecordId.ToString()),
            new Claim("AccessToken", parameters.AccessToken),
            new Claim("SteamId", parameters.SteamId ?? throw new PipelineException("SteamId should exsist here")),
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
