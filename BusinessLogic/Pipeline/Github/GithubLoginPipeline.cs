using BusinessLogic.Pipeline.Shared;
using Domain.Pipeline;

namespace BusinessLogic.Pipeline.Github;

public class GithubLoginPipeline : Pipeline<ILoginPipelineParameters>
{
    public GithubLoginPipeline(
        ValidateGithubOAuthRecordStage validateGithubOAuthRecordStage,
        AppendCookieHeaderStage appendCookieHeaderStage,
        DeriveSuccessRedirectUriStage deriveSuccessRedirectUriStage)
    {
        this.AddStage(validateGithubOAuthRecordStage)
            .AddStage(appendCookieHeaderStage)
            .AddStage(deriveSuccessRedirectUriStage);
    }
}