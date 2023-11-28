using BusinessLogic.Pipeline.LoginSuccess;
using Domain.Pipeline;

namespace BusinessLogic.Pipeline;

public class LoginSuccessPipeline : Pipeline<LoginSuccessPipelineParameters>
{
    public LoginSuccessPipeline(
        ValidateOAuthRecordStage validateOAuthRecordStage,
        AppendCookieHeaderStage appendCookieHeaderStage,
        DeriveSuccessRedirectUriStage deriveSuccessRedirectUriStage)
    {
        this.AddStage(validateOAuthRecordStage)
            .AddStage(appendCookieHeaderStage)
            .AddStage(deriveSuccessRedirectUriStage);
    }
}
