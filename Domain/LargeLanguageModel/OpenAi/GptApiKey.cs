using Domain.LargeLanguageModel.Shared;

namespace Domain.LargeLanguageModel.OpenAi;

public class GptApiKey : LargeLanguageModelReservableApiKey
{
    public GptApiKey(string apiKey, Func<LargeLanguageModelReservableApiKey, Task> unlockAction)
        : base(apiKey, unlockAction)
    {
    }
}