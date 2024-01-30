using Domain.Abstractions;

namespace Domain.Exception;

public class UnhandledResultErrorException : System.Exception
{
    public UnhandledResultErrorException(Error error)
    {
        this.Error = error;
    }

    public Error Error { get; init; }
}
