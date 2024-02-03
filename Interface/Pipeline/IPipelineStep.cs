using Domain.Abstractions;

namespace Interface.Pipeline;

public interface IPipelineStep<T>
{
    Task<Result<T>> Execute(
        T context,
        CancellationToken cancellationToken);
}
