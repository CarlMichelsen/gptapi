using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.OpenAi;

public class GptChoice : GptChoiceBase
{
    [JsonPropertyName("message")]
    public required GptReceivedMessage Message { get; init; }
}
