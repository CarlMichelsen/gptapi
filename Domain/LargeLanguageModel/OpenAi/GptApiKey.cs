using Domain.LargeLanguageModel.Shared;

namespace Domain.LargeLanguageModel.OpenAi;

public class GptApiKey : LlmReservableApiKey
{
    public GptApiKey(string apiKey, Func<LlmReservableApiKey, Task> unlockAction)
        : base(apiKey, unlockAction)
    {
    }
}