namespace Domain.Gpt;

public class GptApiKey : IAsyncDisposable
{
    private readonly Func<GptApiKey, Task> unlockAction;
    private bool disposed = false;

    public GptApiKey(string apiKey, Func<GptApiKey, Task> unlockAction)
    {
        this.ApiKey = apiKey;
        this.unlockAction = unlockAction;
    }

    public string? ApiKey { get; set; }

    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsyncCore();

        this.RegisterDispose(false);
        GC.SuppressFinalize(this);
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
