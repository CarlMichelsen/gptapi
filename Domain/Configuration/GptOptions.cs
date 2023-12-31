﻿namespace Domain.Configuration;

public class GptOptions
{
    public const string SectionName = "Gpt";

    /// <summary>
    /// Gets or sets list of GPT api keys.
    /// These values are stored as a single string in the configuration.
    /// The values are comma-separated in that string.
    /// </summary>
    public required List<string> ApiKeys { get; set; }
}
