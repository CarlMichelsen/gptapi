using Domain.LargeLanguageModel.Shared.Stream;

namespace Domain.LargeLanguageModel.Shared.Interface;

public interface ILlmChunkConvertible
{
    LlmChunk Convert(Guid streamIdentifier);
}
