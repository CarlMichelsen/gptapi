namespace Domain.Exception;

public class ApplicationStartupException : System.Exception
{
    public ApplicationStartupException(string message)
        : base(message)
    {
    }

    public ApplicationStartupException(string? message, System.Exception? innerException)
        : base(message, innerException)
    {
    }
}