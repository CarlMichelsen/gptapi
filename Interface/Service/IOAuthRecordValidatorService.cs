using Domain;
using Domain.Entity;
using Domain.Entity.Id;
using Domain.Service;

namespace Interface.Service;

public interface IOAuthRecordValidatorService
{
    Task<Result<OAuthRecordValidatorResult, string>> ValidateOAuthRecord(
        OAuthRecordId oAuthRecordId,
        string accessToken,
        AuthMethods validAuthenticationMethod);
}
