using Domain.Dto.Steam;

namespace Interface.Handler;

public interface IDevelopmentIdpHandler
{
    Task<DevelopmentIdpResponse> GetDevelopmentUsers();
}
