using Domain.LargeLanguageModel.Shared.Response;

namespace Domain.LargeLanguageModel.Shared.Interface;

public interface ILlmResponseConvertible
{
    LlmResponse Convert();
}
