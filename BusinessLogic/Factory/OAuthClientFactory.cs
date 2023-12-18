using BusinessLogic.Client;
using Domain.Entity;
using Domain.Exception;
using Interface.Client;
using Interface.Factory;

namespace BusinessLogic.Factory;

public class OAuthClientFactory : IOAuthClientFactory
{
    private readonly IServiceProvider serviceProvider;

    public OAuthClientFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }
    
    public IOAuthClient Create(AuthMethods authenticationMethod)
    {
        switch (authenticationMethod)
        {
            case AuthMethods.Development: return this.GetService<DevelopmentOAuthClient>();
            case AuthMethods.Steam: return this.GetService<SteamOAuthClient>();
            case AuthMethods.Github: return this.GetService<GithubOAuthClient>();
            default: throw new OAuthException("Unsupported authentication method");
        }
    }

    private T GetService<T>()
        where T : class
    {
        var type = typeof(T);
        var obj = this.serviceProvider.GetService(type);
        if (obj is null)
        {
            throw new ClientException($"Failed to create {type.Name}");
        }

        return (obj as T) ?? throw new ClientException("Somehow the service became null after a null-check???");
    }
}
