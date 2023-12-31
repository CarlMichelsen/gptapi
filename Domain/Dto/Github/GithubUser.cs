﻿using System.Text.Json.Serialization;

namespace Domain.Dto.Github;

public class GithubUser : GithubSimpleUser
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}
