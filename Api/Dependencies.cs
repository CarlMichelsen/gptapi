﻿using Api.Extensions;
using Api.Security;
using BusinessLogic.Client;
using BusinessLogic.Factory;
using BusinessLogic.Handler;
using BusinessLogic.Pipeline.SendMessage;
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
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Api;

public static class Dependencies
{
    public static void RegisterApplicationDependencies(this WebApplicationBuilder builder)
    {
        // Configuration
        builder.Configuration.AddJsonFile("secrets.json", optional: false, reloadOnChange: true);
        builder.Services
            .AddCors()
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
            .AddTransient<IGptApiKeyProvider, GptApiKeyProvider>()
            .AddTransient<ICacheService, CacheService>()
            .AddTransient<ISessionService, SessionService>();

        builder.Services.AddSignalR();

        // Cache
        var redisConfiguration = builder.Configuration.GetSection(RedisOptions.SectionName);
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfiguration[nameof(RedisOptions.ConnectionString)];
            options.InstanceName = redisConfiguration[nameof(RedisOptions.InstanceName)];
        });

        // Pipelines
        builder.Services
            .RegisterPipelineStages()
            .AddTransient<SendMessagePipeline>();

        // Handlers
        builder.Services
            .AddTransient<ISessionHandler, SessionHandler>()
            .AddTransient<IConversationHandler, ConversationHandler>();

        // Factories
        builder.Services
            .AddTransient<IScopedServiceFactory, ScopedServiceFactory>()
            .AddTransient<IConversationTemplateFactory, ConversationTemplateFactory>();
        
        // Access Control
        builder.Services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = GptApiConstants.SessionAuthenticationScheme;
            options.DefaultChallengeScheme = GptApiConstants.SessionAuthenticationScheme;
        })
        .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(GptApiConstants.SessionAuthenticationScheme, options => { });
        
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(GptApiConstants.SessionAuthenticationScheme, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimsConstants.UserProfileId);
                policy.RequireClaim(ClaimsConstants.AuthenticationMethodUserId);
                policy.RequireClaim(ClaimsConstants.Name);
                policy.RequireClaim(ClaimsConstants.Email);
                policy.RequireClaim(ClaimsConstants.AuthenticationMethod);
                policy.RequireClaim(ClaimsConstants.AuthenticationMethodName);
            });
        });

        // Typed HttpClient Factories
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient<GptChatClient>(client =>
            client.Timeout = TimeSpan.FromSeconds(30));
    }
}