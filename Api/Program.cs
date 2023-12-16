using Api.Endpoints;
using Api.Extensions;
using BusinessLogic.Client;
using BusinessLogic.Factory;
using BusinessLogic.Handler;
using BusinessLogic.Handler.OAuth.Development;
using BusinessLogic.Handler.OAuth.Github;
using BusinessLogic.Handler.OAuth.Steam;
using BusinessLogic.Hub;
using BusinessLogic.Pipeline.Development;
using BusinessLogic.Pipeline.Github;
using BusinessLogic.Pipeline.SendMessage;
using BusinessLogic.Pipeline.Steam;
using BusinessLogic.Provider;
using BusinessLogic.Service;
using Database;
using Domain;
using Domain.Configuration;
using Interface.Client;
using Interface.Factory;
using Interface.Handler;
using Interface.Handler.OAuth;
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
    .Configure<GptOptions>((options) =>
    {
        options.ApiKeys = builder.Configuration.GetListFromConfiguration(
            GptOptions.SectionName,
            nameof(options.ApiKeys));
    })
    .Configure<SteamOAuthOptions>(builder.Configuration.GetSection(SteamOAuthOptions.SectionName))
    .Configure<GithubOAuthOptions>(builder.Configuration.GetSection(GithubOAuthOptions.SectionName))
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
    .AddTransient<IOAuthRecordValidatorService, OAuthRecordValidatorService>()
    .AddScoped<IDevelopmentIdentityProvider, DevelopmentIdentityProvider>()
    .AddTransient<IGptApiKeyProvider, GptApiKeyProvider>()
    .AddTransient<IEndpointUrlProvider, EndpointUrlProvider>();

builder.Services.AddSignalR();

// Pipelines
builder.Services
    .RegisterPipelineStages()
    .AddSingleton<SendMessagePipelineSingleton>()
    .AddTransient<DevelopmentLoginPipeline>()
    .AddTransient<GithubLoginPipeline>()
    .AddTransient<SteamLoginFailurePipeline>()
    .AddTransient<SteamLoginSuccessPipeline>();

// Handlers
builder.Services
    .AddTransient<ISessionHandler, SessionHandler>()
    .AddTransient<IConversationHandler, ConversationHandler>()
    .AddTransient<SteamOAuthLoginSuccessHandler>()
    .AddTransient<DevelopmentOAuthLoginSuccessHandler>()
    .AddTransient<SteamOAuthLoginFailureHandler>();

builder.Services
    .AddTransient<SteamOAuthLoginHandler>()
    .AddTransient<GithubOAuthLoginHandler>()
    .AddTransient<GithubOAuthLoginSuccessHandler>()
    .AddTransient<DevelopmentOAuthLoginHandler>();

// Factories
builder.Services
    .AddTransient<SteamOAuthClient>()
    .AddTransient<GithubOAuthClient>()
    .AddTransient<DevelopmentOAuthClient>();
builder.Services
    .AddTransient<IOAuthClientFactory, OAuthClientFactory>()
    .AddTransient<IConversationTemplateFactory, ConversationTemplateFactory>();

// Typed HttpClient Factories
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<GptChatClient>(client =>
    client.Timeout = TimeSpan.FromSeconds(30));
builder.Services.AddHttpClient(GptApiConstants.GithubHttpClient, options => options.BaseAddress = new Uri("https://github.com"));
builder.Services.AddHttpClient(GptApiConstants.GithubAPIHttpClient, options => options.BaseAddress = new Uri("https://api.github.com"));

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
        policy.WithOrigins(GptApiConstants.DeveloperFrontendUrl)
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
    .MapHealthCheckEndpoints()
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