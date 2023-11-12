namespace Interface.Hubs;

public interface IChatServer
{
    Task SendMessage(string user, string message);
}
