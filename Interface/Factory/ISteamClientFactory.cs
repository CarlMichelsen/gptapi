using Interface.Client;

namespace Interface.Factory;

public interface ISteamClientFactory
{
    ISteamClient Create();
}
