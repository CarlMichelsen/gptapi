namespace Domain.Exception;

public class LargeLanguageModelException : System.Exception
{
    public LargeLanguageModelException(string message)
        : base(message)
    {
    }

    public LargeLanguageModelException(string? message, System.Exception? innerException)
        : base(message, innerException)
    {
    }
}
