namespace Domain.Exception;

public class SessionException : System.Exception
{
    public SessionException(string message)
        : base(message)
    {
    }
}
