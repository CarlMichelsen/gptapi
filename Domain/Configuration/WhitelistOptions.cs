namespace Domain.Configuration;

public class WhitelistOptions
{
    public const string SectionName = "Whitelist";

    /// <summary>
    /// Gets or sets list of whitelisted steamids.
    /// These values are stored as a single string in the configuration.
    /// The values are comma-separated in that string.
    /// </summary>
    public required List<string> WhitelistedSteamIds { get; set; }
}
