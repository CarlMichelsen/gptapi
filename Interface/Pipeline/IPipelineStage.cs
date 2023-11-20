namespace Interface.Pipeline;

public interface IPipelineStage
{
}

public interface IPipelineStage<TInput, TOutput> : IPipelineStage
{
    Task<TOutput> Process(TInput input);
}
