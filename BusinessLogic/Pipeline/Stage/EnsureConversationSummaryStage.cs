using BusinessLogic.Hub;
using BusinessLogic.Map;
using Domain.Entity;
using Domain.Exception;
using Domain.Gpt;
using Domain.Pipeline;
using Interface.Client;
using Interface.Hub;
using Interface.Pipeline;
using Interface.Service;
using Microsoft.AspNetCore.SignalR;

namespace BusinessLogic.Pipeline.Stage;

public class EnsureConversationSummaryStage : IPipelineStage<SendMessagePipelineParameter>
{
    private readonly IGptChatClient gptChatClient;
    private readonly IConversationService conversationService;
    private readonly IHubContext<ChatHub, IChatClient> chatHub;

    public EnsureConversationSummaryStage(
        IGptChatClient gptChatClient,
        IConversationService conversationService,
        IHubContext<ChatHub, IChatClient> chatHub)
    {
        this.gptChatClient = gptChatClient;
        this.conversationService = conversationService;
        this.chatHub = chatHub;
    }

    public async Task<SendMessagePipelineParameter> Process(
        SendMessagePipelineParameter input,
        CancellationToken cancellationToken)
    {
        var conv = input.Conversation
            ?? throw new PipelineException("Conversation should be defined at this point");
        if (!string.IsNullOrWhiteSpace(conv.Summary))
        {
            return input;
        }

        // I am not checking if the client is still connected...
        var client = this.chatHub.Clients.Client(input.ConnectionId);

        var prompt = this.GetGptConversationSummaryPrompt(conv);
        var res = await this.gptChatClient.Prompt(prompt, cancellationToken)
            ?? throw new PipelineException("Conversation summary GptChatClient request returned null");
        var summary = this.GetSummaryFromGptResponse(res);
        
        var success = await this.conversationService.SetConversationSummary(
            input.UserId,
            conv.Id,
            summary);
        
        if (success)
        {
            await client.AssignSummaryToConversation(conv.Id, summary);
        }

        return input;
    }

    private GptChatPrompt GetGptConversationSummaryPrompt(Conversation conv)
    {
        // This won't get saved anywhere. I'm just using it to map to a GptChatPrompt.
        var conversation = new Conversation
        {
            Id = Guid.Empty,
            UserId = string.Empty,
            Summary = null,
            Messages = new()
            {
                new Message
                {
                    Role = Role.System,
                    Visible = false,
                    Content = "Keep track of what is being said so you can make a summary of the conversation later.",
                    Created = DateTime.UtcNow,
                },
            },
            Created = DateTime.UtcNow,
        };

        var relevantMessages = conv.Messages.Where(m => m.Visible);
        conversation.Messages.AddRange(relevantMessages);

        var finalUserPrompt = new Message
        {
            Role = Role.User,
            Visible = false,
            Content = "Give me a short summary of our conversation so far.",
            Created = DateTime.UtcNow,
        };

        var finalInstruction = new Message
        {
            Role = Role.System,
            Visible = false,
            Content = "Respond with a short summary (max 50 characters)",
            Created = DateTime.UtcNow,
        };

        conversation.Messages.Add(finalInstruction);
        conversation.Messages.Add(finalUserPrompt);

        return GptMapper.Map(conversation);
    }

    private string GetSummaryFromGptResponse(GptChatResponse response)
    {
        return response.Choices.First().Message.Content;
    }
}
