using Microsoft.AspNetCore.Http;

namespace Interface.Handler.OAuth;

public interface IOAuthLoginHandler
{
    Task<IResult> Login();
}
