namespace Domain.Entity;

[Flags]
public enum AuthenticationMethod
{
    /// <summary>
    /// User authenticated using Development OAuth, this should not happen in production.
    /// </summary>
    Development = 1,

    /// <summary>
    /// User authenticated using Steam OAuth.
    /// </summary>
    Steam = 2,

    /// <summary>
    /// User authenticated using Github OAuth.
    /// </summary>
    Github = 4,
}
