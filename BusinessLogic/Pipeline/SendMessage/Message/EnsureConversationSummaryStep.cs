using BusinessLogic.Hub;
using BusinessLogic.Map;
using Database;
using Domain.Abstractions;
using Domain.Entity;
using Domain.LargeLanguageModel.Shared;
using Domain.LargeLanguageModel.Shared.Request;
using Domain.Pipeline.SendMessage;
using Interface.Client;
using Interface.Hub;
using Interface.Pipeline;
using Microsoft.AspNetCore.SignalR;

namespace BusinessLogic.Pipeline.SendMessage.Message;

public class EnsureConversationSummaryStep : IPipelineStep<SendMessagePipelineContext>
{
    private readonly ApplicationContext applicationContext;
    private readonly ILlmClient largeLanguageModelClient;
    private readonly IHubContext<ChatHub, IChatClient> chatHub;

    public EnsureConversationSummaryStep(
        ApplicationContext applicationContext,
        ILlmClient largeLanguageModelClient,
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

        var prompt = this.GeneratePrompt(context.Conversation!, "gpt-4", 8192);
        var res = await this.largeLanguageModelClient.Prompt(prompt, LlmProvider.OpenAi, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return new Error("EnsureConversationSummaryStep.Cancelled");
        }

        if (res.IsError)
        {
            return res.Error!;
        }

        var result = res.Unwrap().Convert().Choices.FirstOrDefault()?.Content;
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

    private LlmRequest GeneratePrompt(Conversation conversation, string model, int maxTokens)
    {
        var systemMsg = "You need to keep track of what is being said in this conversation. At the end you will be asked to do a summary. Do your absolute best to stay within the parameters given to you.";
        var initialPrompt = LargeLanguageModelMapper.Map(conversation, model, systemMsg, maxTokens);

        initialPrompt.Messages.Add(new LlmMessage
        {
            Role = LargeLanguageModelMapper.Map(Role.User),
            Content = "Respond with a 6 to 8 word summary of the full conversation preceding this message. Write the summary in first person as if you're the assistant. Ignore system messages.",
        });

        return initialPrompt;
    }
}
