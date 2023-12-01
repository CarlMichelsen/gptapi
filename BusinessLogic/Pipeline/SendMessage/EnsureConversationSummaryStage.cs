using System.Text.RegularExpressions;
using BusinessLogic.Hub;
using BusinessLogic.Map;
using Domain.Entity;
using Domain.Exception;
using Domain.Gpt;
using Domain.Pipeline;
using Interface.Client;
using Interface.Factory;
using Interface.Hub;
using Interface.Pipeline;
using Interface.Service;
using Microsoft.AspNetCore.SignalR;

namespace BusinessLogic.Pipeline.SendMessage;

/// <summary>
/// Pretty sure this class is only partial because of the regex implementation.
/// </summary>
public partial class EnsureConversationSummaryStage : IPipelineStage<SendMessagePipelineParameters>
{
    private const string FinalSystemMessage = @"Respond with a short description of the conversation so far (max 80 characters).
        Make sure the description is memorable so the conversation can be identified by it later.
        The description will be used as a title for the conversation. Don't use special characters.";
    
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

    [GeneratedRegex(@"\s+")]
    private static partial Regex MyRegex();

    private string GetSummaryFromGptResponse(GptChatResponse response)
    {
        return response.Choices.First().Message.Content;
    }
}
