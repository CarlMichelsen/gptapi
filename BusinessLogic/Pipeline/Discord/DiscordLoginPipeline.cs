using BusinessLogic.Pipeline.Discord.Login;
using BusinessLogic.Pipeline.Shared;
using Domain.Pipeline;

namespace BusinessLogic.Pipeline.Discord;

public class DiscordLoginPipeline : Pipeline<ILoginPipelineParameters>
{
    public DiscordLoginPipeline(
        ValidateDiscordOAuthRecordStage validateDiscordOAuthRecordStage,
        AppendCookieHeaderStage appendCookieHeaderStage,
        DeriveSuccessRedirectUriStage deriveSuccessRedirectUriStage)
    {
        this.AddStage(validateDiscordOAuthRecordStage)
            .AddStage(appendCookieHeaderStage)
            .AddStage(deriveSuccessRedirectUriStage);
    }
}
