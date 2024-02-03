using Domain.Abstractions;
using Interface.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic.Factory;

public class ScopedServiceFactory : IScopedServiceFactory
{
    private readonly IServiceProvider serviceProvider;

    public ScopedServiceFactory(
        IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public Result<T> CreateScopedService<T>()
    {
        return this.CreateServiceInNewScope<T>();
    }

    private Result<T> CreateServiceInNewScope<T>()
    {
        var scope = this.serviceProvider.CreateScope();
        var serviceObj = scope.ServiceProvider.GetService(typeof(T));
        if (serviceObj is null)
        {
            return new Error("ScopedServiceFactory.ServiceNotFound");
        }

        return (T)serviceObj;
    }
}
