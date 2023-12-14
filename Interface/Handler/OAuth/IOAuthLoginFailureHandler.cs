using Domain.Entity.Id;
using Microsoft.AspNetCore.Http;

namespace Interface.Handler.OAuth;

public interface IOAuthLoginFailureHandler
{
    Task<IResult> LoginFailure(
        OAuthRecordId oAuthRecordId,
        string error,
        CancellationToken cancellationToken);
}
