using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api;

[Authorize(AuthenticationSchemes = GptApiAuthenticationScheme.AccessTokenAuthentication)]
public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await this.Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
