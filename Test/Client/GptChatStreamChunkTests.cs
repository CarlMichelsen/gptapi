using System.Text.Json;
using Domain.Converter;
using Domain.Gpt;

namespace Test.Client;

public class GptChatStreamChunkTests
{
    private readonly JsonSerializerOptions options;

    public GptChatStreamChunkTests()
    {
        this.options = new JsonSerializerOptions
        {
            Converters = { new UnixDateTimeConverter() },
        };
    }

    [Fact]
    public void Created_SerializesToUnixTimestamp()
    {
        // Arrange
        var chunk = new GptChatStreamChunk
        {
            Id = "123",
            Object = "object",
            Created = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc), // Example date
            Model = "model",
            Choices = new List<GptStreamChoice>(),
        };

        // Act
        var json = JsonSerializer.Serialize(chunk, this.options);

        // Assert
        var expectedTimestamp = ((DateTimeOffset)chunk.Created).ToUnixTimeSeconds().ToString();
        Assert.Contains($"\"created\":{expectedTimestamp}", json);
    }

    [Fact]
    public void Serialize_DateTimeAsUnixTimestamp()
    {
        // Arrange
        var dateTime = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedTimestamp = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();

        // Act
        var json = JsonSerializer.Serialize(dateTime, this.options);

        // Assert
        Assert.Contains(expectedTimestamp.ToString(), json);
    }

    [Fact]
    public void Deserialize_UnixTimestampAsDateTime()
    {
        // Arrange
        var expectedDateTime = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var unixTimestamp = ((DateTimeOffset)expectedDateTime).ToUnixTimeSeconds();

        // Act
        var deserializedDateTime = JsonSerializer.Deserialize<DateTime>(unixTimestamp.ToString(), this.options);

        // Assert
        Assert.Equal(expectedDateTime, deserializedDateTime);
    }
}
