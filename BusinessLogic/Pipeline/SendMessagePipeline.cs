using BusinessLogic.Pipeline.Stage;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic.Pipeline;

public sealed class SendMessagePipeline : Pipeline<SendMessagePipelineParameter>
{
    private readonly List<Type> stageTypes = new();

    private readonly IServiceProvider serviceProvider;

    public SendMessagePipeline(
        IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        this.AddStage(typeof(CreateOrAppendConversationStage))
            .AddStage(typeof(NotifyUserOfCreatedMessageStage))
            .AddStage(typeof(StreamChatResponseStage))
            .AddStage(typeof(RegisterMessageResponseStage))
            .AddStage(typeof(EnsureConversationSummaryStage));
    }

    public override Task<SendMessagePipelineParameter> Execute(SendMessagePipelineParameter input, CancellationToken cancellationToken)
    {
        // Don't forget to clear!
        this.ClearStages();

        var scopedServiceProvider = this.serviceProvider.CreateScope().ServiceProvider;

        foreach (var stageType in this.stageTypes)
        {
            var obj = scopedServiceProvider.GetRequiredService(stageType);
            if (obj is null)
            {
                throw new PipelineException(
                    $"Failed to instantiate PipelineStage of type {stageType.Name}");
            }

            var stage = (IPipelineStage<SendMessagePipelineParameter>)obj;
            this.AddStage(stage);
        }

        return base.Execute(input, cancellationToken);
    }

    private SendMessagePipeline AddStage(Type pipelineStage)
    {
        if (pipelineStage.IsAssignableFrom(typeof(IPipelineStage<SendMessagePipelineParameter>)))
        {
            throw new PipelineException(
                "Attempted to register a pipelineStage that is does not implement IPipelineStage");
        }

        this.stageTypes.Add(pipelineStage);
        return this;
    }
}
