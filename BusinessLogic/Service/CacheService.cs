﻿using BusinessLogic.Json;
using Interface.Service;
using Microsoft.Extensions.Caching.Distributed;

namespace BusinessLogic.Service;

public class CacheService : ICacheService
{
    private readonly IDistributedCache distributedCache;

    public CacheService(
        IDistributedCache distributedCache)
    {
        this.distributedCache = distributedCache;
    }

    public Task<string?> Get(string key)
    {
        return this.distributedCache.GetStringAsync(key);
    }

    public async Task<T?> GetJson<T>(string key)
    {
        var value = await this.Get(key);
        if (value is null)
        {
            return default;
        }

        return CamelCaseJsonParser.Deserialize<T>(value);
    }

    public Task RemoveAsync(string key)
    {
        return this.distributedCache.RemoveAsync(key);
    }

    public Task Set(string key, string value, DistributedCacheEntryOptions? distributedCacheEntryOptions = null)
    {
        if (distributedCacheEntryOptions is null)
        {
            return this.distributedCache.SetStringAsync(key, value);
        }

        return this.distributedCache.SetStringAsync(key, value, distributedCacheEntryOptions);
    }

    public async Task SetJson<T>(string key, T value, DistributedCacheEntryOptions? distributedCacheEntryOptions = null)
    {
        var strValue = CamelCaseJsonParser.Serialize(value);
        if (strValue is null)
        {
            return;
        }

        await this.Set(key, strValue, distributedCacheEntryOptions);
    }
}
