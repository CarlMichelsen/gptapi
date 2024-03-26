using Domain.Abstractions;
using Domain.Dto;
using Domain.Dto.Session;
using Interface.Handler;
using Interface.Service;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Handler;

public class SessionHandler : ISessionHandler
{
    private readonly ISessionService sessionService;

    public SessionHandler(ISessionService sessionService)
    {
        this.sessionService = sessionService;
    }

    public async Task<IResult> GetUserData()
    {
        var sessionDataResult = await this.sessionService.GetSessionData();
        if (sessionDataResult.IsError)
        {
            var badRes = new ServiceResponse<UserDto>("Unauthorized");
            return Results.Ok(badRes);
        }

        var sessionData = sessionDataResult.Unwrap();
        var dto = new UserDto
        {
            Id = sessionData.User.Id,
            Name = sessionData.User.Name,
            AvatarUrl = sessionData.User.AvatarUrl,
        };

        var okRes = new ServiceResponse<UserDto>(dto);
        return Results.Ok(okRes);
    }
}
