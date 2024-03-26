using Domain.Dto.Session;
using Domain.Exception;
using Interface.Service;

namespace BusinessLogic.Handler;

public abstract class BaseHandler
{
    private readonly ISessionService sessionService;

    protected BaseHandler(
        ISessionService sessionService)
    {
        this.sessionService = sessionService;
    }

    protected async Task<SessionData> GetSession()
    {
        var sessionResult = await this.sessionService.GetSessionData();
        if (sessionResult.IsError)
        {
            throw new SessionException($"Failed to get session when the user should be logged in: {sessionResult.Error!.Code}");
        }

        return sessionResult.Unwrap();
    }
}
