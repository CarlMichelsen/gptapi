using Domain.Entity;
using Interface.Client;

namespace Interface.Factory;

public interface IOAuthClientFactory
{
    IOAuthClient Create(AuthMethods authenticationMethod);
}
