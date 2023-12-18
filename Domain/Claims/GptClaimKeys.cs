using System.Security.Claims;

namespace Domain.Claims;

public static class GptClaimKeys
{
    /// <summary>
    /// UserProfile identifier.
    /// </summary>
    public const string UserProfileId = ClaimTypes.Name;

    /// <summary>
    /// The type of OAuth service used for login.
    /// This value is derived from the name of this enum <see cref="Entity.AuthMethods" />.
    /// </summary>
    public const string AuthenticationMethod = "AuthenticationMethod";

    /// <summary>
    /// AuthenticationId from OAuth.
    /// </summary>
    public const string AuthenticationId = "AuthenticationId";

    /// <summary>
    /// Id of the OAuthRecord that the user logged in with.
    /// </summary>
    public const string OAuthRecordId = "OAuthRecordId";
}
