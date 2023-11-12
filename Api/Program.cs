using Api;
using BusinessLogic;
using BusinessLogic.Client;
using Domain.Configuration;
using Interface;
using Interface.Client;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration
builder.Configuration.AddJsonFile("secrets.json", optional: true, reloadOnChange: true);
builder.Services
    .AddCors()
    .Configure<GptOptions>(builder.Configuration.GetSection(GptOptions.SectionName))
    .Configure<AccessOptions>(builder.Configuration.GetSection(AccessOptions.SectionName))
    .AddTransient<IGptApiKeyProvider, GptApiKeyProvider>()
    .AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, AccessTokenAuthenticationHandler>(GptApiAuthenticationScheme.AccessTokenAuthentication, null);

// Services
builder.Services.AddTransient<IGptChatClient, GptChatClient>();
builder.Services.AddSignalR();

// Typed HttpClient Factories
builder.Services.AddHttpClient<GptChatClient>(client =>
    client.Timeout = TimeSpan.FromMinutes(10));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Frontend origin for development
    app.UseCors(policy => 
        policy.WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<ChatHub>("/chathub");

app.MapPromptEndpoints();

app.UseStaticFiles();

app.MapFallbackToFile("index.html");

app.Run();