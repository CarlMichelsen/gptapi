namespace Domain.Dto.LargeLanguageModel;

public class AvailableModelResponse
{
    public required Dictionary<string, List<AvailableModel>> AvailableModels { get; init; }
}