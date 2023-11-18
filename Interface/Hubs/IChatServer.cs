namespace Interface.Hubs;

public interface IChatServer
{
    Task SendMessage(string messageContent, Guid? conversationId = null);
}
