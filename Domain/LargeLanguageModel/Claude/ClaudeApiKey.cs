using Domain.LargeLanguageModel.Shared;

namespace Domain.LargeLanguageModel.Claude;

public class ClaudeApiKey : LargeLanguageModelReservableApiKey
{
    public ClaudeApiKey(string apiKey, Func<LargeLanguageModelReservableApiKey, Task> unlockAction)
        : base(apiKey, unlockAction)
    {
    }
}