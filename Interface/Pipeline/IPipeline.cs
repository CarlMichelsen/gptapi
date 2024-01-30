using Domain.Abstractions;

namespace Interface.Pipeline;

public interface IPipeline<T>
{
    Task<Result<T>> Execute(
        T input,
        CancellationToken cancellationToken);
}
