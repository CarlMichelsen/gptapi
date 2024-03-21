using Domain.Abstractions;
using Domain.Pipeline.SendMessage;

namespace Domain.Pipeline;

public class PipelineTracker
{
    public Guid PipelineIdentifier { get; init; }

    public CancellationTokenSource CancellationTokenSource { get; init; }

    public Task<Result<SendMessagePipelineContext>> PipelineTask { get; init; }
}
