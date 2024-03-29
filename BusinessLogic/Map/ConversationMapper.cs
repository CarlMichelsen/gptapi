﻿using Domain.Abstractions;
using Domain.Dto.Conversation;
using Domain.Entity;
using Domain.Exception;
using Interface.Map;

namespace BusinessLogic.Map;

public class ConversationMapper : IConversationMapper
{
    public static ConversationOptionDto MapToMetaDataDto(
        Conversation conversation)
    {
        return new ConversationOptionDto(
            conversation.Id.Value,
            conversation.Summary ?? "New conversation",
            conversation.LastAppendedUtc,
            conversation.CreatedUtc);
    }

    public static string Map(Role role)
    {
        return Enum.GetName(role)?.ToLower()
            ?? throw new MapException("Message role enum failed to map properly");
    }

    public static MessageDto Map(
        Message message)
    {
        return new MessageDto(
            message.Id.Value,
            message.PreviousMessage?.Id.Value,
            Map(message.Role),
            message.Content ?? throw new MapException("Message content null when mapping message"),
            message.CompletedUtc,
            message.CreatedUtc,
            message.Usage is null ? null : Map(message.Usage),
            message.Visible);
    }

    public static UsageDto Map(Usage usage)
    {
        return new UsageDto
        {
            Provider = Enum.GetName(usage.Provider)?.ToLower() ?? throw new MapException("Unable to map LlmProvider to string"),
            Model = usage.Model,
            Tokens = usage.Tokens,
        };
    }
    
    public Task<Result<ConversationDto>> MapConversation(
        Conversation conversation)
    {
        var conv = new ConversationDto(
            conversation.Id.Value,
            conversation.Summary,
            this.MapMessages(conversation.Messages),
            conversation.LastAppendedUtc);
        
        return Task.FromResult<Result<ConversationDto>>(conv);
    }

    private static void AssignMessageToContainerRecursive(
        int index,
        Message message,
        List<Message> allMessages,
        List<MessageContainer> messageContainers)
    {
        var msgDto = Map(message);
        var messageContainer = messageContainers.ElementAtOrDefault(index);
        if (messageContainer is null)
        {
            messageContainer = new MessageContainer
            {
                Index = index,
                MessageOptions = new Dictionary<Guid, MessageDto>(),
                SelectedMessage = msgDto.Id,
            };
            messageContainers.Add(messageContainer);
        }

        if (!messageContainer!.MessageOptions.TryAdd(msgDto.Id, msgDto))
        {
            return;
        }

        var nextMessages = allMessages
            .Where(m => m.PreviousMessage is not null)
            .Where(m => m.PreviousMessage!.Id == message.Id)
            .ToList();

        if (nextMessages.Count == 0)
        {
            return;
        }

        foreach (var nextMessage in nextMessages)
        {
            AssignMessageToContainerRecursive(
                index + 1,
                nextMessage,
                allMessages,
                messageContainers);
        }
    }

    private List<MessageContainer> MapMessages(List<Message> messages)
    {
        var messageContainers = new List<MessageContainer>();
        var firstMessage = messages
            .SingleOrDefault(m => m.PreviousMessage is null);
        
        if (firstMessage is not null)
        {
            AssignMessageToContainerRecursive(
                0,
                firstMessage,
                messages,
                messageContainers);
        }

        // default sorting
        messageContainers.ForEach(mc =>
        {
            var latestMessage = mc.MessageOptions
                .OrderBy(mo => mo.Value.CreatedUtc)
                .LastOrDefault();
            
            mc.SelectedMessage = latestMessage.Value.Id;
        });

        return messageContainers;
    }
}
