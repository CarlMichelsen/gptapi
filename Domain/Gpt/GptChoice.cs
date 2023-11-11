using System.Text.Json.Serialization;

namespace Domain.Gpt;

public class GptChoice : GptChoiceBase
{
    [JsonPropertyName("message")]
    public required GptReceivedMessage Message { get; init; }
}
