using System.Net;
using Domain;
using Domain.Dto.Steam;

namespace Interface.Handler;

public interface ISessionHandler
{
    Task<Result<SteamPlayerDto, HttpStatusCode>> GetUserData();

    Task<Result<bool, HttpStatusCode>> Logout();
}
