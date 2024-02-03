using Domain.Abstractions;
using Interface.Pipeline;

namespace Interface.Factory;

public interface IScopedServiceFactory
{
    Result<T> CreateScopedService<T>();
}
