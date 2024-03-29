﻿using Domain.Configuration;
using Microsoft.Extensions.Options;

namespace Test;

public static class TestUtil
{
    public static IOptions<GptOptions> CreateGptOptions(List<string> apiKeys)
    {
        var options = Options.Create(new GptOptions { ApiKeys = apiKeys });
        return options;
    }
}
