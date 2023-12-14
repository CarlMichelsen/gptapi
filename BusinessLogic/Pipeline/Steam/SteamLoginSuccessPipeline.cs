using BusinessLogic.Pipeline.Steam.LoginSuccess;
using Domain.Pipeline;

namespace BusinessLogic.Pipeline.Steam;

public class SteamLoginSuccessPipeline : Pipeline<LoginSuccessPipelineParameters>
{
    public SteamLoginSuccessPipeline(
        ValidateOAuthRecordStage validateOAuthRecordStage,
        AppendCookieHeaderStage appendCookieHeaderStage,
        DeriveSuccessRedirectUriStage deriveSuccessRedirectUriStage)
    {
        this.AddStage(validateOAuthRecordStage)
            .AddStage(appendCookieHeaderStage)
            .AddStage(deriveSuccessRedirectUriStage);
    }
}
