namespace Domain.Dto.LargeLanguageModel;

public class AvailableModel
{
    public required string DisplayName { get; init; }
    
    public required string ProviderIdentifier { get; init; }

    public required string ModelIdentifier { get; init; }
    
    public required int MaxTokens { get; init; }
    
    public required string? Description { get; init; }
}