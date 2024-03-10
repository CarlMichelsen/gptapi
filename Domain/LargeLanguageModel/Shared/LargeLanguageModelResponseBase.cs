namespace Domain.LargeLanguageModel.Shared;

public abstract class LargeLanguageModelResponseBase
{
    public required string Id { get; init; }

    public required string Object { get; init; }

    public required DateTime Created { get; init; }

    public required string ModelName { get; init; }

    public string? ResponseFingerprint { get; init; }
}
