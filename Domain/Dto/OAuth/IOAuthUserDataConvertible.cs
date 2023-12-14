namespace Domain.OAuth;

public interface IOAuthUserDataConvertible
{
    OAuthUserData ToOAuthUser();
}
