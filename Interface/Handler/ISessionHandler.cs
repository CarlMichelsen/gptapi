using System.Net;
using Domain;
using Domain.OAuth;

namespace Interface.Handler;

public interface ISessionHandler
{
    Task<Result<OAuthUserData, HttpStatusCode>> GetUserData();

    Task<Result<bool, HttpStatusCode>> Logout();
}
