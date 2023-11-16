using Domain.Gpt;
using Interface.Client;
using Microsoft.AspNetCore.Mvc;

namespace Api;

public static class PromptEndpoints
{
    public static RouteGroupBuilder MapPromptEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/Prompt", async ([FromServices] IGptChatClient gptClient, CancellationToken token) =>
        {
            var prompt = new GptChatPrompt
            {
                Model = "gpt-4",
                Messages = new List<GptChatMessage>
                {
                    new GptChatMessage
                    {
                        Role = "system",
                        Content = "Be extremely passive aggressive",
                    },
                    new GptChatMessage
                    {
                        Role = "user",
                        Content = "Tell me a short story of max 10 words!",
                    },
                },
            };

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(4));
                throw new OperationCanceledException("This endpoint is disabled.");

                var res = await gptClient.Prompt(prompt, token);
                return Results.Ok(res);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine();
                Console.Write("Cancelled prompt!");
            }

            return Results.StatusCode(429);
        })
        .WithName("Prompt")
        .WithOpenApi();

        return group;
    }
}
