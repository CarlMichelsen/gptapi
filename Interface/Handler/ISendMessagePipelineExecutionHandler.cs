using Domain.Abstractions;
using Domain.Pipeline.SendMessage;

namespace Interface.Handler;

public interface ISendMessagePipelineExecutionHandler
{
    Guid SendMessage(SendMessagePipelineContext context, Action<Error> errorHandler);

    void CancelMessage(Guid pipelineIdentifier);
}
