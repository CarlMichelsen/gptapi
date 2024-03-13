using BusinessLogic.Hub;
using BusinessLogic.Map;
using Database;
using Domain.Abstractions;
using Domain.Entity;
using Domain.LargeLanguageModel.Shared;
using Domain.Pipeline.SendMessage;
using Interface.Client;
using Interface.Hub;
using Interface.Pipeline;
using Microsoft.AspNetCore.SignalR;

namespace BusinessLogic.Pipeline.SendMessage.Message;

public class EnsureConversationSummaryStep : IPipelineStep<SendMessagePipelineContext>
{
    private readonly ApplicationContext applicationContext;
    private readonly ILargeLanguageModelClient largeLanguageModelClient;
    private readonly IHubContext<ChatHub, IChatClient> chatHub;

    public EnsureConversationSummaryStep(
        ApplicationContext applicationContext,
        ILargeLanguageModelClient largeLanguageModelClient,
        IHubContext<ChatHub, IChatClient> chatHub)
    {
        this.applicationContext = applicationContext;
        this.largeLanguageModelClient = largeLanguageModelClient;
        this.chatHub = chatHub;
    }

    public async Task<Result<SendMessagePipelineContext>> Execute(
        SendMessagePipelineContext context,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(context.Conversation!.Summary))
        {
            return context;
        }

        var prompt = this.GeneratePrompt(context.Conversation!);
        var res = await this.largeLanguageModelClient.Prompt(prompt, LargeLanguageModelProvider.Claude, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return new Error("EnsureConversationSummaryStep.Cancelled");
        }

        if (res.IsError)
        {
            return res.Error!;
        }

        var result = res.Unwrap().Convert().Options.FirstOrDefault()?.Message.Content;
        if (result is null)
        {
            return new Error("EnsureConversationSummaryStep.NoSummary");
        }

        context.Conversation!.Summary = result;

        var client = this.chatHub.Clients.Client(context.ConnectionId);
        if (client is null)
        {
            return new Error(
                "CompleteGptResponseMessageStep.NoClient",
                "Unable to access signalR client");
        }

        await client.AssignSummaryToConversation(context.Conversation!.Id.Value, context.Conversation!.Summary);

        await this.applicationContext.SaveChangesAsync();
        return context;
    }

    private LargeLanguageModelRequest GeneratePrompt(Conversation conversation)
    {
        var initialPrompt = LargeLanguageModelMapper.Map(conversation, "claude-3-opus-20240229"); // gpt-4

        initialPrompt.Messages.Add(new LargeLanguageModelMessage
        {
            Role = LargeLanguageModelMapper.Map(Role.User),
            Content = "Respond with a 4 to 6 word summary of the full conversation preceding this message. Write the summary in first person as if you're the assistant. Ignore system messages.",
        });

        return initialPrompt;
    }
}
