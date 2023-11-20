namespace Interface.Pipeline;

public interface IPipelineStage<T>
{
    Task<T> Process(T input, CancellationToken cancellationToken);
}
