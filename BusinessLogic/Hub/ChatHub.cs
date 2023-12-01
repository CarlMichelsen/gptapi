using System.Security.Claims;
using BusinessLogic.Handler;
using BusinessLogic.Pipeline;
using Domain.Claims;
using Domain.Context;
using Domain.Entity.Id;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Hub;

[Authorize]
public class ChatHub : ChatHubHandler
{
    private readonly ILogger<ChatHub> logger;

    public ChatHub(
        ILogger<ChatHub> logger,
        ILogger<ChatHubHandler> handlerLogger,
        SendMessagePipelineSingleton sendMessagePipeline)
        : base(handlerLogger, sendMessagePipeline)
    {
        this.logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var httpContext = this.Context.GetHttpContext();
            var claims = httpContext!.User.Claims.ToList();

            var userProfileGuid = Guid.Parse(claims.First(c => c.Type == GptClaimKeys.UserProfileId).Value);
            var oAuthRecordIdGuid = Guid.Parse(claims.First(c => c.Type == GptClaimKeys.OAuthRecordId).Value);

            var context = new GptClaims
            {
                UserProfileId = new UserProfileId(userProfileGuid),
                AuthRecordId = new OAuthRecordId(oAuthRecordIdGuid),
                AuthenticationMethod = claims.First(c => c.Type == GptClaimKeys.AuthenticationMethod).Value,
                AuthenticationId = claims.First(c => c.Type == GptClaimKeys.AuthenticationId).Value,
            };

            this.Context.Items.Add("context", context);
        }
        catch (Exception e)
        {
            this.logger.LogCritical("Failed to read data from claims, terminating connection. {e}", e);
            this.Context.Abort();
            return;
        }

        this.logger.LogInformation(
            "Client\t{id}\tconnected ({userProfileId}) |{authenticationMethod}| <{authenticationId}>",
            this.Context.ConnectionId,
            this.GptClaims.UserProfileId,
            this.GptClaims.AuthenticationMethod,
            this.GptClaims.AuthenticationId);
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception is null)
        {
            this.logger.LogInformation(
                "Client\t{id}\tdisconnected ({userProfileId}) |{authenticationMethod}| <{authenticationId}>",
                this.Context.ConnectionId,
                this.GptClaims.UserProfileId,
                this.GptClaims.AuthenticationMethod,
                this.GptClaims.AuthenticationId);
        }
        else
        {
            this.logger.LogCritical(
                "Client\t{id}\tdisconnected\t{exception}",
                this.Context.ConnectionId,
                exception);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}