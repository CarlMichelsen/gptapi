using Microsoft.AspNetCore.Http;

namespace Interface.Handler;

public interface ISessionHandler
{
    Task<IResult> GetUserData();
}