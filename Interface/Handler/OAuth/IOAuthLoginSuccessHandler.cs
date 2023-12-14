using Domain.Entity.Id;
using Microsoft.AspNetCore.Http;

namespace Interface.Handler.OAuth;

public interface IOAuthLoginSuccessHandler
{
    Task<IResult> LoginSuccess(
        OAuthRecordId oAuthRecordId,
        string tokenType,
        string accessToken,
        CancellationToken cancellationToken);
}
