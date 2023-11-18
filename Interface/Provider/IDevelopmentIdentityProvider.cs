using Domain.Dto.Steam;

namespace Interface.Provider;

public interface IDevelopmentIdentityProvider
{
    Task<DevelopmentIdpResponse> GetDevelopmentUsers();
}
