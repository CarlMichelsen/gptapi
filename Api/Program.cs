using BusinessLogic.Client;
using Domain.Configuration;
using Domain.Gpt;
using Interface.Client;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration
builder.Configuration.AddJsonFile("secrets.json", optional: false, reloadOnChange: true);

builder.Services.Configure<GptOptions>(builder.Configuration.GetSection(GptOptions.SectionName));

// Clients
builder.Services.AddTransient<IGptChatClient, GptChatClient>();

// Typed HttpClient Factories
builder.Services.AddHttpClient<GptChatClient>(client =>
    client.Timeout = TimeSpan.FromMinutes(10));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/prompt", async ([FromServices] IGptChatClient gptClient) =>
{
    var prompt = new GptChatPrompt
    {
        Model = "gpt-4",
        Messages = new List<GptChatMessage>
        {
            new GptChatMessage
            {
                Role = "system",
                Content = "Only do nice things!",
            },
            new GptChatMessage
            {
                Role = "user",
                Content = "Tell me a short story of max 50 words!",
            },
        },
    };

    var token = new CancellationTokenSource();
    //token.CancelAfter(TimeSpan.FromSeconds(5));

    try
    {
        var res = await gptClient.Prompt(prompt, token.Token);
        Console.WriteLine(res.Choices.First().Message.Content);
        /*await foreach (var chunk in gptClient.StreamPrompt(prompt, token.Token))
        {
            Console.Write(chunk.Choices.First().Delta.Content);
        }*/
    }
    catch (System.OperationCanceledException)
    {
        Console.WriteLine();
        Console.Write("Cancelled prompt!");
    }
})
.WithName("Prompt")
.WithOpenApi();

app.Run();