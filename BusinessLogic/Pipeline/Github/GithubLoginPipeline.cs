using Domain.Pipeline;

namespace BusinessLogic.Pipeline.Github;

public class GithubLoginPipeline : Pipeline<GithubLoginSuccessPipelineParameters>
{
    public GithubLoginPipeline()
    {
        //this.AddStage();
    }
}