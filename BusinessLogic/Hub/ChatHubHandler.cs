using System.Text;
using BusinessLogic.Map;
using Database;
using Domain.Context;
using Domain.Dto.Conversation;
using Domain.Entity;
using Interface.Client;
using Interface.Hubs;
using Interface.Service;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Hub;

public class ChatHubHandler : Hub<IChatClient>, IChatServer
{
    private readonly ILogger<ChatHubHandler> logger;
    private readonly IConversationService conversationService;
    private readonly IServiceProvider serviceProvider;
    private readonly IGptChatClient gptChatClient;

    public ChatHubHandler(
        ILogger<ChatHubHandler> logger,
        IConversationService conversationService,
        IServiceProvider serviceProvider,
        IGptChatClient gptChatClient)
    {
        this.logger = logger;
        this.conversationService = conversationService;
        this.serviceProvider = serviceProvider;
        this.gptChatClient = gptChatClient;
    }

    protected ChatHubContext ChatHubContext
    {
        get => (ChatHubContext)this.Context.Items["context"]!;
    }

    public async Task SendMessage(string messageContent, Guid? conversationId = null)
    {
        using (var scope = this.serviceProvider.CreateScope())
        {
            var userId = this.ChatHubContext.SteamId.ToString();
            var message = this.CreateMessage(messageContent, Role.User);
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            Domain.Result<Conversation, string> conversationResult;
            if (conversationId is null || conversationId == Guid.Empty)
            {
                conversationResult = await this.conversationService
                    .StartConversation(dbContext, userId, message);
            }
            else
            {
                var notNullConversationId = Guid.Parse(conversationId!.ToString()!);
                conversationResult = await this.conversationService
                    .AppendConversation(dbContext, userId, notNullConversationId, message);
            }

            var conversation = conversationResult.Match<Conversation?>(
                (conversation) => conversation,
                (error) =>
                {
                    this.logger.LogWarning(
                        "{userId} attempted to append a conversation that does not exsist {error}",
                        userId,
                        error);
                    return null;
                });

            await dbContext.SaveChangesAsync();

            if (conversation is null)
            {
                return;
            }

            // A conversation has been created and appended or just appended by the user at this point.
            var task = conversationId is null
                ? this.Clients.Caller.ReceiveFirstMessage(ConversationMapper.MapFirstMessage(conversation.Id, conversation.Messages.Last()))
                : this.Clients.Caller.ReceiveMessage(ConversationMapper.Map(conversation.Messages.Last()));
            await task;
            
            var prompt = GptMapper.Map(conversation);

            var chunkTasks = new List<Task<MessageChunkDto>>();
            var index = 0;

            var assistantMessage = this.CreateMessage(string.Empty, Role.Assistant);
            conversation.Messages.Add(assistantMessage);
            await dbContext.SaveChangesAsync();

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
                    Role = ConversationMapper.Map(assistantMessage.Role),
                    Content = choice.Delta.Content,
                    Created = messageChunk.Created,
                };
                
                chunkTasks.Add(this.SendChunkToCallerAndReturnIt(messageChunkDto));
                index++;
            }

            assistantMessage.Content = await this.CollectAndSortChunks(chunkTasks);
            await dbContext.SaveChangesAsync();
            await this.Clients.Caller.ReceiveMessage(ConversationMapper.Map(assistantMessage));
        }
    }

    private async Task<string> CollectAndSortChunks(List<Task<MessageChunkDto>> chunkTasks)
    {
        var chunks = await Task.WhenAll(chunkTasks);
        var sb = new StringBuilder();
        foreach (var chunk in chunks.OrderBy(c => c.Index))
        {
            sb.Append(chunk.Content);
        }

        return sb.ToString();
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
            ResponseId = null,
            Role = role,
            Content = messageContent,
        };
    }
}
