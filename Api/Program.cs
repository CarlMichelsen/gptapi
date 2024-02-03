using Api;
using Api.Endpoints;
using Api.Extensions;
using BusinessLogic.Hub;
using Domain;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.RegisterApplicationDependencies();

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
else
{
    app.Services.EnsureDatabaseUpdated();
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<ChatHub>("/chathub");

app.MapGroup("/api/v1")
    .MapConversationEndpoints()
    .MapSessionEndpoints()
    .WithOpenApi();

app.UseStaticFiles();

app.MapFallbackToFile("index.html")
    .WithName(GptApiConstants.FrontendEndpointName);

app.Run();