using BusinessLogic.Client;
using Domain.Configuration;
using Domain.Exception;
using Interface.Client;
using Interface.Factory;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Factory;

public class OAuthClientFactory : IOAuthClientFactory
{
    private readonly IOptions<ApplicationOptions> applicationOptions;
    private readonly IServiceProvider serviceProvider;

    public OAuthClientFactory(
        IOptions<ApplicationOptions> applicationOptions,
        IServiceProvider serviceProvider)
    {
        this.applicationOptions = applicationOptions;
        this.serviceProvider = serviceProvider;
    }
    
    public IOAuthClient Create()
    {
        if (this.applicationOptions.Value.IsDevelopment)
        {
            return this.GetService<DevelopmentOAuthClient>();
        }

        return this.GetService<SteamOAuthClient>();
    }

    private T GetService<T>()
    {
        var type = typeof(T);
        var obj = this.serviceProvider.GetService(type);
        if (obj is null)
        {
            throw new ClientException($"Failed to create {type.Name}");
        }

        return (T)obj;
    }
}
