namespace Domain.Exception;

public class PipelineException : System.Exception
{
    public PipelineException(string message)
        : base(message)
    {
    }

    public PipelineException(string? message, System.Exception? innerException)
        : base(message, innerException)
    {
    }
}
