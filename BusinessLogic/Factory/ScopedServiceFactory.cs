using Domain.Abstractions;
using Interface.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic.Factory;

public class ScopedServiceFactory : IScopedServiceFactory
{
    private readonly IServiceScope scope;

    public ScopedServiceFactory(
        IServiceProvider serviceProvider)
    {
        this.scope = serviceProvider.CreateScope();
    }

    public Result<T> CreateScopedService<T>()
    {
        var serviceObj = this.scope.ServiceProvider.GetService(typeof(T));
        if (serviceObj is null)
        {
            return new Error("ScopedServiceFactory.ServiceNotFound");
        }

        return (T)serviceObj;
    }
}
