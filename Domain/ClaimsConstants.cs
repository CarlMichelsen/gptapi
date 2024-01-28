using System.Security.Claims;

namespace Domain;

public static class ClaimsConstants
{
    public const string UserProfileId = ClaimTypes.NameIdentifier;

    public const string AuthenticationMethodUserId = "AuthenticationMethodUserId";

    public const string Name = ClaimTypes.Name;

    public const string Email = ClaimTypes.Email;

    public const string AuthenticationMethod = "AuthenticationMethod";

    public const string AuthenticationMethodName = "AuthenticationMethodName";
}
