using System.Net;
using Domain;
using Domain.Dto.Session;
using Interface.Handler;
using Interface.Service;

namespace BusinessLogic.Handler;

public class SessionHandler : ISessionHandler
{
    private readonly ISessionService sessionService;

    public SessionHandler(ISessionService sessionService)
    {
        this.sessionService = sessionService;
    }

    public async Task<DeprecatedResult<UserDto, HttpStatusCode>> GetUserData()
    {
        var sessiondata = await this.sessionService.GetSessionData();
        if (sessiondata is null)
        {
            return HttpStatusCode.Unauthorized;
        }

        return new UserDto
        {
            Id = sessiondata.User.Id,
            Name = sessiondata.User.Name,
            AvatarUrl = sessiondata.User.AvatarUrl,
        };
    }
}
