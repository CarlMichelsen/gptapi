using System.Text.Json.Serialization;

namespace Domain.Gpt;

public class GptStreamChoice : GptChoiceBase
{
    [JsonPropertyName("delta")]
    public required GptReceivedMessage Delta { get; init; }
}
