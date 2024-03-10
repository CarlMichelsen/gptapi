using System.Text.Json.Serialization;

namespace Domain.LargeLanguageModel.OpenAi;

public class GptStreamChoice : GptChoiceBase
{
    [JsonPropertyName("delta")]
    public required GptDelta Delta { get; init; }
}
