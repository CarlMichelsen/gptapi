using Domain.LargeLanguageModel.Shared;

namespace Domain.LargeLanguageModel.Shared.Interface;

public interface ILargeLanguageModelResponseConvertible
{
    LargeLanguageModelResponse Convert();
}
