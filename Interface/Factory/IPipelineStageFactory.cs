using Interface.Pipeline;

namespace Interface.Factory;

public interface IPipelineStageFactory
{
    T Create<T>()
        where T : IPipelineStage, new();
}
