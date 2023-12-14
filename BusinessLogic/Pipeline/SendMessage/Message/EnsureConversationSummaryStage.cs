using BusinessLogic.Hub;
using BusinessLogic.Map;
using Domain.Exception;
using Domain.Gpt;
using Domain.Pipeline;
using Interface.Client;
using Interface.Factory;
using Interface.Hub;
using Interface.Pipeline;
using Interface.Service;
using Microsoft.AspNetCore.SignalR;

namespace BusinessLogic.Pipeline.SendMessage.Message;

public class EnsureConversationSummaryStage : IPipelineStage<SendMessagePipelineParameters>
{
    private readonly IGptChatClient gptChatClient;
    private readonly IConversationService conversationService;
    private readonly IConversationTemplateFactory conversationTemplateFactory;
    private readonly IHubContext<ChatHub, IChatClient> chatHub;

    public EnsureConversationSummaryStage(
        IGptChatClient gptChatClient,
        IConversationService conversationService,
        IConversationTemplateFactory conversationTemplateFactory,
        IHubContext<ChatHub, IChatClient> chatHub)
    {
        this.gptChatClient = gptChatClient;
        this.conversationService = conversationService;
        this.conversationTemplateFactory = conversationTemplateFactory;
        this.chatHub = chatHub;
    }

    public async Task<SendMessagePipelineParameters> Process(
        SendMessagePipelineParameters input,
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

        var convToBeMapped = this.conversationTemplateFactory.CreateConversationForSummaryPrompt(conv);
        var prompt = GptMapper.Map(convToBeMapped);
        
        var res = await this.gptChatClient.Prompt(prompt, cancellationToken)
            ?? throw new PipelineException("Conversation summary GptChatClient request returned null");
        var summary = this.GetSummaryFromGptResponse(res).Replace("\"", string.Empty);
        
        var success = await this.conversationService.SetConversationSummary(
            input.UserProfileId,
            conv.Id,
            summary);
        
        if (success)
        {
            await client.AssignSummaryToConversation(conv.Id.Value, summary);
        }

        return input;
    }

    private string GetSummaryFromGptResponse(GptChatResponse response)
    {
        return response.Choices.First().Message.Content;
    }
}
