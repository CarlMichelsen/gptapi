using System.Text;
using BusinessLogic;

namespace Test.Client;

public class JsonStreamProcessorTests
{
    [Theory]
    [InlineData("{\"key\":\"value\"}", new string[] { "{\"key\":\"value\"}" })]
    [InlineData("{\"key1\":\"value1\"}{\"key2\":\"value2\"}", new string[] { "{\"key1\":\"value1\"}", "{\"key2\":\"value2\"}" })]
    [InlineData("RandomData{\"key\":\"value\"}MoreRandomData", new string[] { "{\"key\":\"value\"}" })]
    public async Task ReadJsonObjectsAsync_ReturnsCorrectJsonObjects(string input, string[] expectedJsonObjects)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

        var jsonObjects = await JsonStreamProcessor.ReadJsonObjectsAsync(stream).ToListAsync();

        Assert.Equal(expectedJsonObjects.Length, jsonObjects.Count);
        for (int i = 0; i < expectedJsonObjects.Length; i++)
        {
            Assert.Equal(expectedJsonObjects[i], jsonObjects[i]);
        }
    }

    [Fact]
    public async Task ReadJsonObjectsAsync_HandlesIncompleteJson()
    {
        string input = "{\"key\":\"value";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

        var jsonObjects = await JsonStreamProcessor.ReadJsonObjectsAsync(stream).ToListAsync();

        Assert.Empty(jsonObjects);
    }

    [Fact]
    public async Task ReadJsonObjectsAsync_HandlesEmptyStream()
    {
        using var stream = new MemoryStream();

        var jsonObjects = await JsonStreamProcessor.ReadJsonObjectsAsync(stream).ToListAsync();

        Assert.Empty(jsonObjects);
    }

    [Fact]
    public async Task ReadJsonObjectsAsync_HandlesNestedJsonObjects()
    {
        string input = "{\"outer\": {\"inner\": \"value\"}}";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

        var jsonObjects = await JsonStreamProcessor.ReadJsonObjectsAsync(stream).ToListAsync();

        Assert.Single(jsonObjects);
        Assert.Equal(input, jsonObjects[0]);
    }

    [Fact]
    public async Task ReadJsonObjectsAsync_HandlesSpecialCharacters()
    {
        string input = "{\"key\": \"Line1\\nLine2\\tTab\"}";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

        var jsonObjects = await JsonStreamProcessor.ReadJsonObjectsAsync(stream).ToListAsync();

        Assert.Single(jsonObjects);
        Assert.Equal(input, jsonObjects[0]);
    }

    [Fact]
    public async Task ReadJsonObjectsAsync_HandlesLargeJsonObjects()
    {
        string largeValue = new string('a', 1024); // Larger than buffer size
        string input = $"{{\"key\": \"{largeValue}\"}}";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

        var jsonObjects = await JsonStreamProcessor.ReadJsonObjectsAsync(stream, 256).ToListAsync();

        Assert.Single(jsonObjects);
        Assert.Equal(input, jsonObjects[0]);
    }

    [Fact]
    public async Task ReadJsonObjectsAsync_HandlesInvalidJson()
    {
        string input = "{\"key\": \"value\"";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

        var jsonObjects = await JsonStreamProcessor.ReadJsonObjectsAsync(stream).ToListAsync();

        Assert.Empty(jsonObjects);
    }
}
