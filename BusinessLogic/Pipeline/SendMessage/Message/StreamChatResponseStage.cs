using System.Text;
using BusinessLogic.Hub;
using BusinessLogic.Map;
using Domain.Dto.Conversation;
using Domain.Entity;
using Domain.Entity.Id;
using Domain.Exception;
using Domain.Pipeline;
using Interface.Client;
using Interface.Hub;
using Interface.Pipeline;
using Microsoft.AspNetCore.SignalR;

namespace BusinessLogic.Pipeline.SendMessage.Message;

public class StreamChatResponseStage : IPipelineStage<SendMessagePipelineParameters>
{
    private readonly IGptChatClient gptChatClient;
    private readonly IHubContext<ChatHub, IChatClient> chatHub;

    public StreamChatResponseStage(
        IGptChatClient gptChatClient,
        IHubContext<ChatHub, IChatClient> chatHub)
    {
        this.gptChatClient = gptChatClient;
        this.chatHub = chatHub;
    }

    public async Task<SendMessagePipelineParameters> Process(
        SendMessagePipelineParameters input,
        CancellationToken cancellationToken)
    {
        // I am not checking if the client is still connected...
        var client = this.chatHub.Clients.Client(input.ConnectionId);
        var conv = input.Conversation
            ?? throw new PipelineException("Conversation should be defined at this point");

        var prompt = GptMapper.Map(conv);
        string? responseId = null;
        
        var chunkTasks = new List<Task<MessageChunkDto>>();
        var index = 0;

        await foreach (var messageChunk in this.gptChatClient.StreamPrompt(prompt, cancellationToken))
        {
            if (responseId is null)
            {
                if (messageChunk.Id is null)
                {
                    throw new PipelineException("Response from GptChatClient does not contain a ResponseId, this is bad.");
                }

                responseId = messageChunk.Id;
            }

            var choice = messageChunk.Choices.FirstOrDefault();
            if (choice is null)
            {
                continue;
            }

            var messageChunkDto = new MessageChunkDto
            {
                ConversationId = conv.Id.Value,
                Index = index,
                Role = ConversationMapper.Map(Role.Assistant),
                Content = choice.Delta.Content,
                Created = messageChunk.Created,
            };

            chunkTasks.Add(this.SendChunkToCallerAndReturnIt(client, messageChunkDto));
            index++;

            if (input.StopFurtherMessageStreaming)
            {
                input.ResponseMessage = new Domain.Entity.Message
                {
                    Id = new MessageId(Guid.NewGuid()),
                    Role = Role.Assistant,
                    Content = await this.CollectAndSortChunks(chunkTasks),
                    ResponseId = responseId,
                    Created = DateTime.UtcNow,
                };
                return input;
            }
        }

        input.ResponseMessage = new Domain.Entity.Message
        {
            Id = new MessageId(Guid.NewGuid()),
            Role = Role.Assistant,
            Content = await this.CollectAndSortChunks(chunkTasks),
            ResponseId = responseId,
            Created = DateTime.UtcNow,
        };

        return input;
    }

    private async Task<MessageChunkDto> SendChunkToCallerAndReturnIt(IChatClient client, MessageChunkDto messageChunkDto)
    {
        await client.ReceiveMessageChunk(messageChunkDto);
        return messageChunkDto;
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
}
