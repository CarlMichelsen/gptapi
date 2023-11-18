namespace Domain.Exception;

public class ClientException : System.Exception
{
    public ClientException(string message)
        : base(message)
    {
    }
}
