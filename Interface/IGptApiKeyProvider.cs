namespace Interface;

public interface IGptApiKeyProvider
{
    public Task UnlockAll();

    public Task<string?> ReserveAKey();

    public Task CancelKeyReservation(string key);
}
