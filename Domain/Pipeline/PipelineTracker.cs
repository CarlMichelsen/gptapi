using Domain.Abstractions;
using Domain.Pipeline.SendMessage;

namespace Domain.Pipeline;

public class PipelineTracker
{
    public required Guid PipelineIdentifier { get; init; }

    public required CancellationTokenSource CancellationTokenSource { get; init; }

    public required Task<Result<SendMessagePipelineContext>> PipelineTask { get; init; }
}
