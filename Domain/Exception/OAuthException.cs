namespace Domain.Exception;

public class OAuthException : System.Exception
{
    public OAuthException(string message)
        : base(message)
    {
    }
}
