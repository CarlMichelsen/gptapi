using System.Text.Json;

namespace BusinessLogic.Json;

public static class CamelCaseJsonParser
{
    public static JsonSerializerOptions Default { get; } = new JsonSerializerOptions(JsonSerializerOptions.Default)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static T? Deserialize<T>(string? jsonText)
    {
        try
        {
            if (jsonText is null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(jsonText, Default);
        }
        catch (Exception)
        {
            return default;
        }
    }

    public static string? Serialize(object? obj)
    {
        try
        {
            return JsonSerializer.Serialize(obj, Default);
        }
        catch (Exception)
        {
            return default;
        }
    }
}
