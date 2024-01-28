namespace Interface.Pipeline;

public interface IPipelineStep<T>
{
    Task<T> Process(T input, CancellationToken cancellationToken);
}
