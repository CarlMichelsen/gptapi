using Domain.LargeLanguageModel.Shared;

namespace Domain.LargeLanguageModel.Claude;

public class ClaudeApiKey : LlmReservableApiKey
{
    public ClaudeApiKey(string apiKey, Func<LlmReservableApiKey, Task> unlockAction)
        : base(apiKey, unlockAction)
    {
    }
}