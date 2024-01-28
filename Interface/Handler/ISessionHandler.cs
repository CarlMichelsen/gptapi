using System.Net;
using Domain;
using Domain.Dto.Session;

namespace Interface.Handler;

public interface ISessionHandler
{
    Task<Result<UserDto, HttpStatusCode>> GetUserData();
}