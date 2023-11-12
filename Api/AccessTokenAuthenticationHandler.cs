using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Domain.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Api;

public class AccessTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string AccessTokenQueryParameterName = "access_token";

    private readonly IOptions<AccessOptions> accessOptions;

    public AccessTokenAuthenticationHandler(
        IOptions<AccessOptions> accessOptions,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
        this.accessOptions = accessOptions;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Extract token from query string
        var accessToken = this.Context.Request.Query[AccessTokenQueryParameterName];

        if (string.IsNullOrEmpty(accessToken))
        {
            return AuthenticateResult.Fail("No access token provided");
        }

        try
        {
            var credentialBytes = Convert.FromBase64String(accessToken!);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
            (var username, var password) = (credentials[0], credentials[1]);
            
            if (await this.CheckCredentials(username, password))
            {
                var claims = new[] { new Claim(ClaimTypes.Name, username) };
                var identity = new ClaimsIdentity(claims, this.Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, this.Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.Fail("Invalid username or password");
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Authorization header");
        }
    }

    private Task<bool> CheckCredentials(string username, string password)
    {
        if (this.accessOptions.Value.Users is null)
        {
            return Task.FromResult(false);
        }

        var user = this.accessOptions.Value.Users
            .FirstOrDefault(u => u.Username == username && u.Password == password);

        return Task.FromResult(user is not null);
    }
}
