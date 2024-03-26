using Domain.Abstractions;
using Domain.Dto.Session;

namespace Interface.Service;

public interface ISessionService
{
    Task<Result<SessionData>> GetSessionData();
}
