using BusinessLogic.Provider;
using Domain.Exception;

namespace Test;

[Collection("Sequential")]
public class GptApiKeyProviderTests
{
    [Fact]
    public async Task ReserveAKey()
    {
        // Arrange
        var apiKeys = new List<string> { "key1" };
        var optionsMock = TestUtil.CreateGptOptions(apiKeys);
        var apiKeyProvider = new GptApiKeyProvider(optionsMock);
        await apiKeyProvider.UnsafeUnreserveAll();
        
        // Act
        var keyResult = await apiKeyProvider.GetReservedApiKey();
        var key = keyResult.Unwrap();
        
        // Assert
        Assert.NotNull(key);
        Assert.NotNull(key.ApiKey);
        await key.DisposeAsync();
    }
    
    [Fact]
    public async Task DontReserveTheSameKeyTwice()
    {
        // Arrange
        var apiKeys = new List<string> { "key1" };
        var optionsMock = TestUtil.CreateGptOptions(apiKeys);
        var apiKeyProvider = new GptApiKeyProvider(optionsMock);
        await apiKeyProvider.UnsafeUnreserveAll();
        
        // Act
        var keyResult1 = await apiKeyProvider.GetReservedApiKey();
        var keyResult2 = await apiKeyProvider.GetReservedApiKey();
        
        // Assert
        Assert.True(keyResult1.IsSuccess);
        
        Assert.True(keyResult2.IsError);
        Assert.Throws<UnhandledResultErrorException>(() => keyResult2.Unwrap());

        await keyResult1.Unwrap().DisposeAsync();
    }
    
    [Fact]
    public async Task AllowMultipleKeysToBeReservedAtOnceButOnlyUniqueReservations()
    {
        // Arrange
        var apiKeys = new List<string> { "key1", "key2" };
        var optionsMock = TestUtil.CreateGptOptions(apiKeys);
        var apiKeyProvider = new GptApiKeyProvider(optionsMock);
        await apiKeyProvider.UnsafeUnreserveAll();
        
        // Act
        var keyResult1 = await apiKeyProvider.GetReservedApiKey();
        var keyResult2 = await apiKeyProvider.GetReservedApiKey();
        var keyResult3 = await apiKeyProvider.GetReservedApiKey();
        
        // Assert
        Assert.True(keyResult1.IsSuccess);
        Assert.True(keyResult2.IsSuccess);
        Assert.NotEqual(keyResult1.Unwrap().ApiKey, keyResult2.Unwrap().ApiKey);
        
        Assert.True(keyResult3.IsError);
        Assert.Throws<UnhandledResultErrorException>(() => keyResult3.Unwrap());

        await keyResult1.Unwrap().DisposeAsync();
        await keyResult2.Unwrap().DisposeAsync();
    }
    
    [Fact]
    public async Task MakeSureKeysCanBeUnreserved()
    {
        // Arrange
        var apiKeys = new List<string> { "key1" };
        var optionsMock = TestUtil.CreateGptOptions(apiKeys);
        var apiKeyProvider = new GptApiKeyProvider(optionsMock);
        await apiKeyProvider.UnsafeUnreserveAll();
        
        // Act, Assert
        var keyResult1 = await apiKeyProvider.GetReservedApiKey();
        var keyResult2 = await apiKeyProvider.GetReservedApiKey();
        
        Assert.True(keyResult1.IsSuccess);
        
        Assert.True(keyResult2.IsError);
        Assert.Throws<UnhandledResultErrorException>(() => keyResult2.Unwrap());

        var previouslyReservedApiKey = keyResult1.Unwrap().ApiKey;
        await keyResult1.Unwrap().DisposeAsync();
        
        var keyResult3 = await apiKeyProvider.GetReservedApiKey();
        Assert.True(keyResult3.IsSuccess);
        
        Assert.Equal(keyResult3.Unwrap().ApiKey, previouslyReservedApiKey);
    }

    [Fact]
    public async Task MakeSureUsingStatementWorks()
    {
        // Arrange
        var apiKeys = new List<string> { "key1" };
        var optionsMock = TestUtil.CreateGptOptions(apiKeys);
        var apiKeyProvider = new GptApiKeyProvider(optionsMock);
        await apiKeyProvider.UnsafeUnreserveAll();
        
        // Act, Assert
        var keyResult1 = await apiKeyProvider.GetReservedApiKey();
        var keyResult2 = await apiKeyProvider.GetReservedApiKey();
        
        Assert.True(keyResult2.IsError);
        Assert.Throws<UnhandledResultErrorException>(() => keyResult2.Unwrap());
        
        await using (var key = keyResult1.Unwrap())
        {
            Assert.Equal(key.ApiKey, apiKeys.First());
        }
        
        // Make sure the key is available again after using.
        var keyResult3 = await apiKeyProvider.GetReservedApiKey();
        Assert.True(keyResult3.IsSuccess);
    }
}
