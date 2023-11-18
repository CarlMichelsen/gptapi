namespace Interface.Provider;

public interface IGptApiKeyProvider
{
    public Task UnlockAll();

    public Task<string?> ReserveAKey();

    public Task CancelKeyReservation(string key);
}
