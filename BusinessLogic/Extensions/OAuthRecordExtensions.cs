using Domain.Entity;

namespace BusinessLogic;

public static class OAuthRecordExtensions
{
    public static bool IsCompleted(this OAuthRecord record)
    {
        if (record is
        {
            ReturnedFromThirdParty: not null,
            UserId: not null,
            AccessToken: not null,
            ReturnedFromThirdParty: not null,
        })
        {
            return true;
        }
        
        return false;
    }
}
