using Domain.Dto.Session;

namespace Interface.Service;

public interface ISessionService
{
    Task<SessionData?> GetSessionData();
}
