namespace Interface.Hubs;

public interface IChatServer
{
    Task SendMessage(string messageContent);
}
