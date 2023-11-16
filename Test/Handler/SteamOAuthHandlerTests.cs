using BusinessLogic.Database;
using BusinessLogic.Development;
using BusinessLogic.Handler;
using Domain;
using Domain.Configuration;
using Domain.Entity;
using Interface.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Test;

public class SteamOAuthHandlerTests
{
    private readonly ServiceCollection services;

    public SteamOAuthHandlerTests()
    {
        this.services = TestUtil.GetServiceCollectionWithDatabase();

        var optionsMock = new Mock<IOptions<SteamOAuthOptions>>();
        optionsMock.Setup(o => o.Value).Returns(new SteamOAuthOptions { ClientId = "TestClientId", OAuthEndpoint = null });
        this.services.AddSingleton(optionsMock.Object);

        var steamClientMock = new Mock<ISteamClient>();
        this.services.AddSingleton(steamClientMock.Object);

        var loggerMock = new Mock<ILogger<SteamOAuthHandler>>();
        this.services.AddSingleton(loggerMock.Object);

        this.services.AddSingleton<SteamOAuthHandler>();
    }

    [Fact]
    public async Task SteamLogin_RedirectsToSteamAndSavesRecord()
    {
        // Arrange
        var steamClientMock = new Mock<ISteamClient>();
        this.services.AddSingleton(steamClientMock.Object);

        var loggerMock = new Mock<ILogger<SteamOAuthHandler>>();
        loggerMock.Setup(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()))
                .Verifiable();
        this.services.AddSingleton(loggerMock.Object);

        var serviceProvider = this.services.BuildServiceProvider();
        var context = serviceProvider.GetService<ApplicationContext>()!;
        var handler = serviceProvider.GetService<SteamOAuthHandler>()!;

        // Act
        var result = await handler.SteamLogin();

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.RedirectToRouteHttpResult>(result);
        loggerMock.Verify();
        
        Assert.Single(await context.OAuthRecords.ToListAsync());
    }

    [Fact]
    public async Task SteamLogin_WithEmptyClientId_ThrowsException()
    {
        var optionsMock = new Mock<IOptions<SteamOAuthOptions>>();
        optionsMock.Setup(o => o.Value).Returns(new SteamOAuthOptions { ClientId = string.Empty, OAuthEndpoint = "http://test.com" });
        this.services.AddSingleton(optionsMock.Object);

        var serviceProvider = this.services.BuildServiceProvider();
        var handler = serviceProvider.GetService<SteamOAuthHandler>()!;

        await Assert.ThrowsAsync<NullReferenceException>(() => handler.SteamLogin());
    }

    [Fact]
    public async Task SteamLoginSuccess_WithValidRecord_UpdatesRecord()
    {
        // Arrange
        var developmentUser = DevelopmentIdpHandler.GenerateSteamDevelopmentPlayer("Nicki Mirage", TimeSpan.FromHours(5));
        var steamClientMock = new Mock<ISteamClient>();
        steamClientMock
            .Setup(s => s.GetSteamPlayerSummary(It.IsAny<string>()))
            .ReturnsAsync(developmentUser);
        this.services.AddSingleton(steamClientMock.Object);

        var serviceProvider = this.services.BuildServiceProvider();
        var context = serviceProvider.GetService<ApplicationContext>()!;
        var handler = serviceProvider.GetService<SteamOAuthHandler>()!;

        // Create and add a test record to the context
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

        var newAccessToken = "testAccessToken";

        // Act
        var httpContextMock = new Mock<HttpContext>();
        var result = await handler.SteamLoginSuccess(httpContextMock.Object, testRecord.Id, "steam", newAccessToken);

        // Refresh the record from the database
        var updatedRecord = await context.OAuthRecords.FindAsync(testRecord.Id);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.RedirectHttpResult>(result);
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
    public void SteamOAuthRedirectUri_WithValidParameters_GeneratesCorrectUri(string endpoint, string clientId, string recordIdString, string expectedUri)
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SteamOAuthHandler>>();
        loggerMock.Setup(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()))
                .Verifiable();
        this.services.AddSingleton(loggerMock.Object);

        var optionsMock = new Mock<IOptions<SteamOAuthOptions>>();
        optionsMock.Setup(o => o.Value).Returns(new SteamOAuthOptions { ClientId = clientId, OAuthEndpoint = endpoint });
        this.services.AddSingleton(optionsMock.Object);

        var serviceProvider = this.services.BuildServiceProvider();
        var handler = serviceProvider.GetService<SteamOAuthHandler>()!;

        var recordId = Guid.Parse(recordIdString);

        // Act
        var queryParams = new Dictionary<string, string>
        {
            { "response_type", "token" },
            { "client_id", clientId },
            { "state", recordId.ToString() },
        };
        var redirect = handler.ParseQueryParameters(endpoint, queryParams);

        // Assert
        Assert.Equal(expectedUri, redirect);
    }
}