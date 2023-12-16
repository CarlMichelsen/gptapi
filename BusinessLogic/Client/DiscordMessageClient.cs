using System.Text;
using System.Text.Json;
using Domain.Configuration;
using Domain.Dto.Discord;
using Interface.Client;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Client;

public class DiscordMessageClient : IDiscordMessageClient
{
    private readonly HttpClient httpClient;
    private readonly IOptions<DiscordOptions> discordOptions;

    public DiscordMessageClient(
        HttpClient httpClient,
        IOptions<DiscordOptions> discordOptions)
    {
        this.httpClient = httpClient;
        this.discordOptions = discordOptions;
    }

    public async Task<bool> SendMessage(DiscordWebhookMessage message)
    {
        var json = JsonSerializer.Serialize(message);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await this.httpClient.PostAsync(this.discordOptions.Value.WebhookUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        return true;
    }
}
