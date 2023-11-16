using Api;
using BusinessLogic;
using BusinessLogic.Client;
using BusinessLogic.Database;
using BusinessLogic.Development;
using BusinessLogic.Handler;
using Domain;
using Domain.Configuration;
using Interface;
using Interface.Client;
using Interface.Handler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;

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
    .Configure<SteamOAuthOptions>(builder.Configuration.GetSection(SteamOAuthOptions.SectionName))
    .AddTransient<IGptApiKeyProvider, GptApiKeyProvider>()
    .AddAuthentication()
        .AddScheme<AuthenticationSchemeOptions, AccessTokenAuthenticationHandler>(
            GptApiConstants.AccessTokenAuthentication, null);

// Database
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseInMemoryDatabase("ApplicationDatabase"));

// Services
builder.Services.AddTransient<IGptChatClient, GptChatClient>();
builder.Services.AddSignalR();

if (env.IsDevelopment())
{
    builder.Services
        .AddTransient<ISteamClient, DevelopmentSteamClient>()
        .AddScoped<IDevelopmentIdpHandler, DevelopmentIdpHandler>();
}
else
{
    builder.Services.AddTransient<ISteamClient, SteamClient>();
}

// Handlers
builder.Services.AddTransient<ISteamOAuthHandler, SteamOAuthHandler>();

// Typed HttpClient Factories
builder.Services.AddHttpClient<GptChatClient>(client =>
    client.Timeout = TimeSpan.FromMinutes(10));

var app = builder.Build();

// Configure the HTTP request pipeline.
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
    
    var hubContext = app.Services.GetRequiredService<IHubContext<ChatHub>>();
    app.Lifetime.ApplicationStopping.Register(() => hubContext.Clients.All.SendAsync("Disconnect"));
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<ChatHub>(GptApiConstants.ChatHubEndpoint);

app.MapGroup("/api/v1")
    .MapPromptEndpoints()
    .MapSteamOAuthEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapDevelopmentIdpEndpoints();
}

app.UseStaticFiles();

app.MapFallbackToFile("index.html");

app.Run();