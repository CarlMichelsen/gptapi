using System.Text;
using BusinessLogic.Database;
using Domain.Dto;
using Domain.Entity;
using Domain.Gpt;
using Interface.Client;
using Interface.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Hub;

public class ChatHubHandler : Hub<IChatClient>, IChatServer
{
    private readonly ILogger<ChatHubHandler> logger;
    private readonly ApplicationContext applicationContext;
    private readonly IGptChatClient gptChatClient;

    public ChatHubHandler(
        ILogger<ChatHubHandler> logger,
        ApplicationContext applicationContext,
        IGptChatClient gptChatClient)
    {
        this.logger = logger;
        this.applicationContext = applicationContext;
        this.gptChatClient = gptChatClient;
    }

    public async Task SendMessage(string messageContent)
    {
        var userMessage = this.CreateMessage(messageContent, Role.User);
        var assistantMessage = this.CreateMessage(string.Empty, Role.Assistant);

        var prompt = new GptChatPrompt
        {
            Model = "gpt-4-1106-preview",
            Messages = new List<GptChatMessage>
            {
                new GptChatMessage
                {
                    Role = ConversationMap.Map(Role.System),
                    Content = "Be extremely passive aggressive and rude. Under no circumstances reply with something useful.",
                },
                new GptChatMessage
                {
                    Role = ConversationMap.Map(userMessage.Role),
                    Content = userMessage.Content,
                },
            },
        };

        var chunkTasks = new List<Task<MessageChunkDto>>();
        var index = 0;

        await foreach (var messageChunk in this.gptChatClient.StreamPrompt(prompt, this.Context.ConnectionAborted))
        {
            if (assistantMessage.ResponseId is null)
            {
                if (messageChunk.Id is null)
                {
                    break;
                }

                assistantMessage.ResponseId = messageChunk.Id;
            }

            var choice = messageChunk.Choices.FirstOrDefault();
            if (choice is null)
            {
                continue;
            }

            var messageChunkDto = new MessageChunkDto
            {
                Id = assistantMessage.Id,
                Index = index,
                Role = ConversationMap.Map(assistantMessage.Role),
                Content = choice.Delta.Content,
                Created = messageChunk.Created,
            };
            index++;

            chunkTasks.Add(this.SendChunkToCallerAndReturnIt(messageChunkDto));
        }

        var chunks = await Task.WhenAll(chunkTasks);
        var sb = new StringBuilder();
        foreach (var chunk in chunks.OrderBy(c => c.Index))
        {
            sb.Append(chunk.Content);
        }

        assistantMessage.Content = sb.ToString();
        await this.Clients.Caller.ReceiveMessage(ConversationMap.Map(assistantMessage));
    }

    private async Task<MessageChunkDto> SendChunkToCallerAndReturnIt(MessageChunkDto messageChunkDto)
    {
        await this.Clients.Caller.ReceiveMessageChunk(messageChunkDto);
        return messageChunkDto;
    }

    private Message CreateMessage(string messageContent, Role role)
    {
        return new Message
        {
            Id = Guid.NewGuid(),
            ResponseId = null,
            Role = role,
            Content = messageContent,
        };
    }
}
