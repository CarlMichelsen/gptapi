using BusinessLogic;

namespace Test;

public class GptApiKeyProviderTests
{
    [Fact]
    public async Task ReserveAKey_ReturnsKey_WhenAvailable()
    {
        // Arrange
        var apiKeys = new List<string> { "key1", "key2" };
        var optionsMock = TestUtil.CreateGptOptions(apiKeys);
        var apiKeyProvider = new GptApiKeyProvider(optionsMock);
        await apiKeyProvider.UnlockAll();

        // Act
        var reservedKey = await apiKeyProvider.ReserveAKey();

        // Assert
        Assert.NotNull(reservedKey);
        Assert.Contains(reservedKey, apiKeys);
    }

    [Fact]
    public async Task ReserveAKey_ReturnsNull_WhenNoKeysAvailable()
    {
        // Arrange
        var apiKeys = new List<string>(); // No keys available
        var optionsMock = TestUtil.CreateGptOptions(apiKeys);
        var apiKeyProvider = new GptApiKeyProvider(optionsMock);
        await apiKeyProvider.UnlockAll();

        // Act
        var reservedKey = await apiKeyProvider.ReserveAKey();

        // Assert
        Assert.Null(reservedKey);
    }

    [Fact]
    public async Task CancelKeyReservation_RemovesKeyFromInUse()
    {
        // Arrange
        var apiKeys = new List<string> { "key1" };
        var optionsMock = TestUtil.CreateGptOptions(apiKeys);
        var apiKeyProvider = new GptApiKeyProvider(optionsMock);
        await apiKeyProvider.UnlockAll();

        // Reserve a key first
        var reservedKey = await apiKeyProvider.ReserveAKey();

        // Act
        await apiKeyProvider.CancelKeyReservation(reservedKey!);

        // Assert
        var anotherReservedKey = await apiKeyProvider.ReserveAKey(); // Should be able to reserve the same key again
        Assert.Equal(reservedKey, anotherReservedKey);
    }

    [Fact]
    public async Task CancelKeyReservation_ReservesKey()
    {
        // Arrange
        var apiKeys = new List<string> { "key1", "key2" };
        var optionsMock = TestUtil.CreateGptOptions(apiKeys);
        var apiKeyProvider = new GptApiKeyProvider(optionsMock);
        await apiKeyProvider.UnlockAll();

        // Act
        var reservedKey1 = await apiKeyProvider.ReserveAKey();
        var reservedKey2 = await apiKeyProvider.ReserveAKey();
        var reservedKey3 = await apiKeyProvider.ReserveAKey();

        // Assert
        Assert.NotNull(reservedKey1);
        Assert.NotNull(reservedKey2);
        Assert.Null(reservedKey3);
    }
}
