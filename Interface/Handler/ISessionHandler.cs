using System.Net;
using Domain;
using Domain.Dto.Steam;
using Microsoft.AspNetCore.Http;

namespace Interface.Handler;

public interface ISessionHandler
{
    Task<Result<SteamPlayerDto, HttpStatusCode>> GetUserData(
        HttpContext httpContext);

    Task<Result<bool, HttpStatusCode>> Logout(
        HttpContext httpContext);
}
