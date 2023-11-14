using BusinessLogic.Database;
using BusinessLogic.Handler;
using Domain.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Test;

public class SteamOAuthHandlerTests
{
    [Fact]
    public async Task SteamLogin_RedirectsToSteamAndSavesRecord()
    {
        // Arrange
        var services = TestUtil.GetServiceCollectionWithDatabase();

        var loggerMock = new Mock<ILogger<SteamOAuthHandler>>();
        services.AddSingleton(loggerMock.Object);

        var optionsMock = new Mock<IOptions<SteamOAuthOptions>>();
        optionsMock.Setup(o => o.Value).Returns(new SteamOAuthOptions { ClientId = "TestClientId", OAuthEndpoint = null });
        services.AddSingleton(optionsMock.Object);
        var serviceProvider = services.BuildServiceProvider();

        var context = serviceProvider.GetService<ApplicationContext>()!;
        var handler = new SteamOAuthHandler(loggerMock.Object, optionsMock.Object, context);

        // Act
        var result = await handler.SteamLogin();

        // Assert
        Assert.IsType<RedirectResult>(result);
        loggerMock.Verify(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        Assert.Single(context.OAuthRecords); // Ensure a record was added
    }
}
