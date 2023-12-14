namespace Domain.Configuration;

public class WhitelistOptions
{
    public const string SectionName = "Whitelist";

    /// <summary>
    /// Gets or sets list of whitelisted userIds.
    /// These values are stored as a single string in the configuration.
    /// The values are comma-separated in that string.
    /// </summary>
    public required List<string> WhitelistedUserIds { get; set; }
}
