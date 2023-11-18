using BusinessLogic.Database;
using BusinessLogic.Handler;
using BusinessLogic.Provider;
using Domain.Configuration;
using Domain.Entity;
using Domain.Exception;
using Interface.Client;
using Interface.Factory;
using Interface.Handler;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Test.Handler;

public class SteamOAuthHandlerTests
{
    private readonly IServiceCollection services;
    private readonly Mock<ILogger<SteamOAuthHandler>> logger = new();
    private readonly Mock<IHttpContextAccessor> httpContextAccessor = new();
    private readonly Mock<IOptions<SteamOAuthOptions>> steamOAuthOptions = new();
    private readonly Mock<IOptions<ApplicationOptions>> applicationOptions = new();
    private readonly Mock<ISteamClientFactory> steamClientFactory = new();
    
    public SteamOAuthHandlerTests()
    {
        var steamClientMock = new Mock<ISteamClient>();

        this.steamOAuthOptions
            .Setup(o => o.Value)
            .Returns(new SteamOAuthOptions { ClientId = "TestClientId", OAuthEndpoint = "https://www.bbc.com/" });

        this.applicationOptions
            .Setup(o => o.Value)
            .Returns(new ApplicationOptions { IsDevelopment = false });

        this.steamClientFactory
            .Setup(s => s.Create())
            .Returns(steamClientMock.Object);
        
        this.services = TestUtil.GetServiceCollectionWithDatabase();
        this.services.AddSingleton(this.logger.Object);
        this.services.AddSingleton(this.httpContextAccessor.Object);
        this.services.AddSingleton(this.steamOAuthOptions.Object);
        this.services.AddSingleton(this.applicationOptions.Object);
        this.services.AddSingleton(this.steamClientFactory.Object);
        this.services.AddTransient<ISteamOAuthHandler, SteamOAuthHandler>();
    }

    [Fact]
    public async Task SteamLogin_RedirectsToSteamAndSavesRecord()
    {
        // Arrange
        this.logger.Setup(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()))
                .Verifiable();
        this.services.AddSingleton(this.logger.Object);

        var serviceProvider = this.services.BuildServiceProvider();
        var context = serviceProvider.GetService<ApplicationContext>()!;
        var handler = serviceProvider.GetService<ISteamOAuthHandler>()!;

        // Act
        var result = await handler.SteamLogin();

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.RedirectHttpResult>(result);
        this.logger.Verify();
        
        Assert.Single(await context.OAuthRecords.ToListAsync());
    }

    [Fact]
    public async Task SteamLoginFailure_WithValidRecord_UpdatesErrorAndReturnsUnauthorized()
    {
        // Arrange
        var oAuthRecordId = Guid.NewGuid();
        var errorMessage = "Login failed due to timeout";

        // Setup a mock OAuthRecord to simulate a failed login
        var testRecord = new OAuthRecord
        {
            Id = oAuthRecordId,
            RedirectedToSteam = DateTime.UtcNow,
            ReturnedFromSteam = null,
            SteamId = null,
            AccessToken = null,
            Error = null,
        };
        this.services.AddTransient<ISteamOAuthHandler, SteamOAuthHandlerWithFakedVirtualMethods>();

        var serviceProvider = this.services.BuildServiceProvider();
        var context = serviceProvider.GetService<ApplicationContext>()!;

        context.OAuthRecords.Add(testRecord);
        await context.SaveChangesAsync();

        var handler = serviceProvider.GetService<ISteamOAuthHandler>()!;

        // Act
        var result = await handler.SteamLoginFailure(oAuthRecordId, errorMessage);
        var updatedRecord = await context.OAuthRecords.FindAsync(oAuthRecordId);

        // Assert
        Assert.NotNull(updatedRecord);
        Assert.Equal(errorMessage, updatedRecord.Error);
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.UnauthorizedHttpResult>(result);
    }

    [Fact]
    public async Task SteamLoginFailure_WithInvalidRecordId_ReturnsNotFound()
    {
        // Arrange
        var invalidOAuthRecordId = Guid.NewGuid();
        var errorMessage = "Invalid record ID";

        var serviceProvider = this.services.BuildServiceProvider();
        var handler = serviceProvider.GetService<ISteamOAuthHandler>()!;

        // Act
        var result = await handler.SteamLoginFailure(invalidOAuthRecordId, errorMessage);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound>(result);
    }

    [Fact]
    public async Task SteamLogin_WithEmptyClientId_ThrowsException()
    {
        // Arrange
        this.steamOAuthOptions
            .Setup(o => o.Value)
            .Returns(new SteamOAuthOptions { ClientId = string.Empty, OAuthEndpoint = "http://test.com" });
        this.services.AddSingleton(this.steamOAuthOptions.Object);
        var serviceProvider = this.services.BuildServiceProvider();
        var handler = serviceProvider.GetService<ISteamOAuthHandler>()!;

        // Act
        // Assert
        await Assert.ThrowsAsync<OAuthException>(() => handler.SteamLogin());
    }

    [Fact]
    public async Task SteamLoginSuccess_WithValidRecord_UpdatesRecord()
    {
        // Arrange
        var developmentUser = DevelopmentIdentityProvider
            .GenerateSteamDevelopmentPlayer("Nicki Mirage", TimeSpan.FromHours(5));
        
        var steamClientMock = new Mock<ISteamClient>();
        steamClientMock
            .Setup(s => s.GetSteamId(It.IsAny<string>()))
            .ReturnsAsync(developmentUser.SteamId);
        this.steamClientFactory
            .Setup(s => s.Create())
            .Returns(steamClientMock.Object);
        this.services.AddSingleton(this.steamClientFactory.Object);
        this.services.AddTransient<ISteamOAuthHandler, SteamOAuthHandlerWithFakedVirtualMethods>();

        var serviceProvider = this.services.BuildServiceProvider();

        // Fake that a login process has been started already
        var context = serviceProvider.GetService<ApplicationContext>()!;
        var handler = serviceProvider.GetService<ISteamOAuthHandler>()!;

        var testRecord = new OAuthRecord
        {
            Id = Guid.NewGuid(),
            RedirectedToSteam = DateTime.UtcNow,
            ReturnedFromSteam = null,
            SteamId = null,
            AccessToken = null,
            Error = null,
        };
        context.OAuthRecords.Add(testRecord);
        await context.SaveChangesAsync();

        // Act
        var newAccessToken = "testAccessToken";
        var result = await handler.SteamLoginSuccess(
            testRecord.Id,
            "steam",
            newAccessToken);
        var updatedRecord = await context.OAuthRecords.FindAsync(testRecord.Id);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.RedirectToRouteHttpResult>(result);
        Assert.NotNull(updatedRecord);
        Assert.NotNull(updatedRecord.SteamId);
        Assert.NotNull(updatedRecord.ReturnedFromSteam);
        Assert.Equal(newAccessToken, updatedRecord.AccessToken);
        Assert.Equal(developmentUser.SteamId, updatedRecord.SteamId);
    }

    [Theory]
    [InlineData("http://test.com", "ClientId1", "12345678-1234-1234-1234-1234567890ab", "http://test.com/?response_type=token&client_id=ClientId1&state=12345678-1234-1234-1234-1234567890ab")]
    [InlineData("http://example.com", "TestClient", "11223344-5566-7788-99aa-bbccddeeff00", "http://example.com/?response_type=token&client_id=TestClient&state=11223344-5566-7788-99aa-bbccddeeff00")]
    [InlineData("https://oauth.example.org", "OAuthClient", "aabbccdd-eeff-0011-2233-445566778899", "https://oauth.example.org/?response_type=token&client_id=OAuthClient&state=aabbccdd-eeff-0011-2233-445566778899")]
    [InlineData("https://auth.test.com", "Client123", "abcdef00-1111-2222-3333-444455556666", "https://auth.test.com/?response_type=token&client_id=Client123&state=abcdef00-1111-2222-3333-444455556666")]
    [InlineData("http://localhost/auth", "LocalClient", "deadbeef-1234-5678-9abc-def012345678", "http://localhost/auth?response_type=token&client_id=LocalClient&state=deadbeef-1234-5678-9abc-def012345678")]
    public void ParseQueryParameters_URIparse(string endpoint, string clientId, string recordIdString, string expectedUri)
    {
        // Arrange
        var queryParams = new Dictionary<string, string>
        {
            { "response_type", "token" },
            { "client_id", clientId },
            { "state", recordIdString },
        };

        // Act
        var redirect = SteamOAuthHandler.ParseQueryParameters(endpoint, queryParams);

        // Assert
        Assert.Equal(expectedUri, redirect);
    }
}
