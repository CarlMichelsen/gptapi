using Domain.Abstractions;
using Domain.Pipeline.SendMessage;
using Interface.Pipeline;

namespace BusinessLogic.Pipeline.SendMessage.Message;

public class ValidateContextStep : IPipelineStep<SendMessagePipelineContext>
{
    public Task<Result<SendMessagePipelineContext>> Execute(
        SendMessagePipelineContext context,
        CancellationToken cancellationToken)
    {
        // TODO: Validate hehe :-)
        return Task.FromResult<Result<SendMessagePipelineContext>>(context);
    }
}
