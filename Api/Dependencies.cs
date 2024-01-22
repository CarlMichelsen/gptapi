using Api.Extensions;
using BusinessLogic.Client;
using BusinessLogic.Factory;
using BusinessLogic.Handler;
using BusinessLogic.Pipeline.SendMessage;
using BusinessLogic.Provider;
using BusinessLogic.Service;
using Database;
using Domain.Configuration;
using Interface.Client;
using Interface.Factory;
using Interface.Handler;
using Interface.Provider;
using Interface.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Api;

public static class Dependencies
{
    public static void RegisterApplicationDependencies(this WebApplicationBuilder builder)
    {
        // Configuration
        builder.Configuration.AddJsonFile("secrets.json", optional: false, reloadOnChange: true);
        builder.Services
            .Configure<GptOptions>(builder.Configuration.GetSection(GptOptions.SectionName))
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
            .AddTransient<IGptApiKeyProvider, GptApiKeyProvider>();

        builder.Services.AddSignalR();

        // Pipelines
        builder.Services
            .RegisterPipelineStages()
            .AddSingleton<SendMessagePipelineSingleton>();

        // Handlers
        builder.Services
            .AddTransient<IConversationHandler, ConversationHandler>();

        // Factories
        builder.Services
            .AddTransient<IConversationTemplateFactory, ConversationTemplateFactory>();

        // Typed HttpClient Factories
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient<GptChatClient>(client =>
            client.Timeout = TimeSpan.FromSeconds(30));

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
    }
}