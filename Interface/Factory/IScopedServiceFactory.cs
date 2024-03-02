using Domain.Abstractions;

namespace Interface.Factory;

public interface IScopedServiceFactory
{
    Result<T> CreateScopedService<T>();
}
