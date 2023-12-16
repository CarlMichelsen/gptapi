using BusinessLogic.Pipeline.Shared;
using Domain.Pipeline;

namespace BusinessLogic.Pipeline.Development;

public class DevelopmentLoginPipeline : Pipeline<ILoginPipelineParameters>
{
    public DevelopmentLoginPipeline(
        ValidateGithubOAuthRecordStage validateOAuthRecordStage,
        AppendCookieHeaderStage appendCookieHeaderStage,
        DeriveSuccessRedirectUriStage deriveSuccessRedirectUriStage)
    {
        this.AddStage(validateOAuthRecordStage)
            .AddStage(appendCookieHeaderStage)
            .AddStage(deriveSuccessRedirectUriStage);
    }
}