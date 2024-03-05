using BusinessLogic.Hub;
using BusinessLogic.Map;
using Database;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Gpt;
using Domain.Pipeline.SendMessage;
using Interface.Client;
using Interface.Hub;
using Interface.Pipeline;
using Microsoft.AspNetCore.SignalR;

namespace BusinessLogic.Pipeline.SendMessage.Message;

public class EnsureConversationSummaryStep : IPipelineStep<SendMessagePipelineContext>
{
    private readonly ApplicationContext applicationContext;
    private readonly IGptChatClient gptChatClient;
    private readonly IHubContext<ChatHub, IChatClient> chatHub;

    public EnsureConversationSummaryStep(
        ApplicationContext applicationContext,
        IGptChatClient gptChatClient,
        IHubContext<ChatHub, IChatClient> chatHub)
    {
        this.applicationContext = applicationContext;
        this.gptChatClient = gptChatClient;
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
        var res = await this.gptChatClient.Prompt(prompt, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return new Error("EnsureConversationSummaryStep.Cancelled");
        }

        if (res.IsError)
        {
            return res.Error!;
        }

        var result = res.Unwrap().Choices.FirstOrDefault()?.Message.Content;
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

    private GptChatPrompt GeneratePrompt(Conversation conversation)
    {
        var initialPrompt = GptMapper.Map(conversation);

        initialPrompt.Messages.Insert(0, new GptChatMessage
        {
            Role = ConversationMapper.Map(Role.System),
            Content = "Following this message you will be a conversation between a person and you. Make sue to keep track of the contents og this conversation as it will be relevant later.",
        });

        initialPrompt.Messages.Add(new GptChatMessage
        {
            Role = ConversationMapper.Map(Role.System),
            Content = "Keep track of what the conversation has been about so far. whatever you do DO NOT ANSWER WITH MORE THAN 44 CHARACTERS!",
        });

        initialPrompt.Messages.Add(new GptChatMessage
        {
            Role = ConversationMapper.Map(Role.User),
            Content = "Respond with a 4 to 6 word summary of the full conversation preceding this message. Write the summary in first person as if you're the assistant. Ignore system messages.",
        });

        return initialPrompt;
    }
}
