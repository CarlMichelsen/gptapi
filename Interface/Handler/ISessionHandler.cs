using System.Net;
using Domain;
using Domain.Dto.Session;

namespace Interface.Handler;

public interface ISessionHandler
{
    Task<DeprecatedResult<UserDto, HttpStatusCode>> GetUserData();
}