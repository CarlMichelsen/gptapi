using Domain.Dto.Discord;

namespace Interface.Client;

public interface IDiscordMessageClient
{
    Task<bool> SendMessage(DiscordWebhookMessage message);
}
