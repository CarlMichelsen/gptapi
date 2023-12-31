﻿using Domain.Dto.Conversation;
using Domain.Entity;
using Domain.Entity.Id;
using Domain.Exception;

namespace BusinessLogic.Map;

public static class ConversationMapper
{
    public static ConversationMetaDataDto MapMetaData(Conversation conversation)
    {
        return new ConversationMetaDataDto
        {
            Id = conversation.Id.Value,
            Summary = conversation.Summary,
            Created = conversation.Created,
        };
    }

    public static FirstMessageDto MapFirstMessage(ConversationId conversationId, Message message)
    {
        return new FirstMessageDto
        {
            ConversationId = conversationId.Value,
            Message = Map(message),
        };
    }

    public static ConversationDto Map(Conversation conversation)
    {
        return new ConversationDto
        {
            Id = conversation.Id.Value,
            Summary = conversation.Summary,
            Messages = conversation.Messages.Where(m => m.Visible).Select(Map).ToList(),
            LastAppended = conversation.LastAppended,
        };
    }

    public static MessageDto Map(Message message)
    {
        return new MessageDto
        {
            Id = message.Id.Value,
            Role = Map(message.Role),
            Content = message.Content,
            Complete = message.Complete,
            Created = message.Created,
        };
    }

    public static string Map(Role role)
    {
        return Enum.GetName(role)?.ToLower() ?? throw new MapException("Message role enum failed to map properly");
    }
}
