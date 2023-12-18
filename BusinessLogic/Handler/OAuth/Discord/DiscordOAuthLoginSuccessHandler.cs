using Domain.Entity.Id;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Handler.OAuth.Discord;

public class DiscordOAuthLoginSuccessHandler
{
    public Task<IResult> LoginSuccess(
        OAuthRecordId oAuthRecordId,
        string tokenType,
        string accessToken,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
