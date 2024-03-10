using Domain.LargeLanguageModel.Shared;

namespace Domain.LargeLanguageModel.Shared.Interface;

public interface ILargeLanguageModelChunkConvertible
{
    LargeLanguageModelChunk Convert();
}
