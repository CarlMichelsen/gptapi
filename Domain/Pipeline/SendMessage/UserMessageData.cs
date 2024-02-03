namespace Domain.Pipeline.SendMessage;

public record UserMessageData(
    string TemporaryUserMessageId,
    string MessageContent);