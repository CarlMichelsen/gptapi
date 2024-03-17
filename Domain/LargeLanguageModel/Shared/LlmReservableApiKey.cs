namespace Domain.LargeLanguageModel.Shared;

public class LlmReservableApiKey
{
    private readonly Func<LlmReservableApiKey, Task> unlockAction;
    private bool disposed = false;

    public LlmReservableApiKey(
        string apiKey,
        Func<LlmReservableApiKey, Task> unlockAction)
    {
        this.ApiKey = apiKey;
        this.unlockAction = unlockAction;
    }

    public string? ApiKey { get; set; }

    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsyncCore();
        this.RegisterDispose(false);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (this.disposed)
        {
            return;
        }

        await this.unlockAction(this);
        this.ApiKey = null;

        this.disposed = true;
    }

    protected virtual void RegisterDispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
    }
}
