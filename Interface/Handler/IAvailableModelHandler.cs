using Microsoft.AspNetCore.Http;

namespace Interface.Handler;

public interface IAvailableModelHandler
{
    Task<IResult> GetAvailableModels();
}