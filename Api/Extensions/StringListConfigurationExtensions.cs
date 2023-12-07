using Domain.Exception;

namespace Api.Extensions;

public static class StringListConfigurationExtensions
{
    public static List<string> GetListFromConfiguration(
        this IConfiguration configuration,
        string sectionName,
        string propertyName)
    {
        var section = configuration.GetSection(sectionName)
            ?? throw new ApplicationStartupException($"Section \"{sectionName}\" not found in configuration");

        var str = section[propertyName]
            ?? throw new ApplicationStartupException($"Property \"{propertyName}\" not found in section \"{sectionName}\" in configuration");

        return str.Split(',').Select(s => s.Trim()).ToList();
    }
}
