using Api.Endpoints;
using Api.Extensions;
using BusinessLogic.Client;
using BusinessLogic.Factory;
using BusinessLogic.Handler;
using BusinessLogic.Hub;
using BusinessLogic.Pipeline;
using BusinessLogic.Provider;
using BusinessLogic.Service;
using Database;
using Domain;
using Domain.Configuration;
using Interface.Client;
using Interface.Factory;
using Interface.Handler;
using Interface.Provider;
using Interface.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

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
    .Configure<SteamOAuthOptions>(builder.Configuration.GetSection(SteamOAuthOptions.SectionName))
    .Configure<ApplicationOptions>(options => options.IsDevelopment = builder.Environment.IsDevelopment());

// Database
builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        (b) => b.MigrationsAssembly("Api"));

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }
});

// Services
builder.Services
    .AddTransient<IGptChatClient, GptChatClient>()
    .AddTransient<IConversationService, ConversationService>()
    .AddScoped<IDevelopmentIdentityProvider, DevelopmentIdentityProvider>()
    .AddTransient<IGptApiKeyProvider, GptApiKeyProvider>();

builder.Services.AddSignalR();

// Pipelines
builder.Services
    .RegisterPipelineStages()
    .AddSingleton<SendMessagePipelineSingleton>()
    .AddTransient<LoginStartPipeline>()
    .AddTransient<LoginFailurePipeline>()
    .AddTransient<LoginSuccessPipeline>();

// Handlers
builder.Services
    .AddTransient<ISessionHandler, SessionHandler>()
    .AddTransient<IConversationHandler, ConversationHandler>()
    .AddTransient<ISteamOAuthHandler, SteamOAuthHandler>();

// Factories
builder.Services
    .AddTransient<SteamClient>()
    .AddTransient<DevelopmentSteamClient>();
builder.Services
    .AddTransient<ISteamClientFactory, SteamClientFactory>();

// Typed HttpClient Factories
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<GptChatClient>(client =>
    client.Timeout = TimeSpan.FromMinutes(10));

// Security
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.LoginPath = "/";
        options.AccessDeniedPath = "/";
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                // Check if the request is for an API endpoint
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    // Prevent redirection and return 401 Unauthorized
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                }

                // For non-API requests, continue with the default redirection
                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            },
        };
    });

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
    .MapSteamOAuthEndpoints()
    .MapSessionEndpoints()
    .MapConversationEndpoints()
    .WithOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapDevelopmentIdpEndpoints()
        .WithOpenApi();
}

app.UseStaticFiles();

app.MapFallbackToFile("index.html")
    .WithName(GptApiConstants.FrontendEndpointName);

app.Run();