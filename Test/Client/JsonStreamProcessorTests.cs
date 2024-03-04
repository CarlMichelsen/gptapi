using System.Text;
using BusinessLogic.Json;

namespace Test.Client;

public class JsonStreamProcessorTests
{
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
    public async Task ReadJsonObjectsAsync_HandlesInvalidJson()
    {
        string input = "{\"key\": \"value\"";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

        var jsonObjects = await JsonStreamProcessor.ReadJsonObjectsAsync(stream).ToListAsync();

        Assert.Empty(jsonObjects);
    }
}
