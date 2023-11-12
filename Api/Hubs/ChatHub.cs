using Interface.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api;

[Authorize(AuthenticationSchemes = GptApiAuthenticationScheme.AccessTokenAuthentication)]
public class ChatHub : Hub<IChatClient>, IChatServer
{
    public async Task SendMessage(string user, string message)
    {
        await this.Clients.All.ReceiveMessage(user, message);
    }
}
