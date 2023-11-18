using Domain.Entity;

namespace BusinessLogic;

public static class OAuthRecordExtensions
{
    public static bool IsCompleted(this OAuthRecord record)
    {
        if (record is
        {
            ReturnedFromSteam: not null,
            SteamId: not null,
            AccessToken: not null,
            ReturnedFromSteam: not null,
        })
        {
            return true;
        }
        
        return false;
    }
}
