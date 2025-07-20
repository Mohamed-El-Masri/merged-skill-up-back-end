using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SkillUpPlatform.Application.Interfaces;
using System.Text.Json;

namespace SkillUpPlatform.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            if (_cache.TryGetValue(key, out var value))
            {
                if (value is T directValue)
                {
                    return directValue;
                }
                
                if (value is string stringValue)
                {
                    return JsonSerializer.Deserialize<T>(stringValue);
                }
            }
            
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting cache key: {key}");
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        try
        {
            var options = new MemoryCacheEntryOptions();
            
            if (expiry.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiry;
            }
            else
            {
                options.SlidingExpiration = TimeSpan.FromMinutes(30); // Default 30 minutes
            }

            if (value is string || value is ValueType)
            {
                _cache.Set(key, value, options);
            }
            else
            {
                var serializedValue = JsonSerializer.Serialize(value);
                _cache.Set(key, serializedValue, options);
            }
            
            _logger.LogDebug($"Cache set for key: {key}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error setting cache key: {key}");
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            _cache.Remove(key);
            _logger.LogDebug($"Cache removed for key: {key}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error removing cache key: {key}");
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            return _cache.TryGetValue(key, out _);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking cache existence for key: {key}");
            return false;
        }
    }

    public async Task ClearAsync()
    {
        try
        {
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Clear();
            }
            
            _logger.LogInformation("Cache cleared");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache");
        }
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
    {
        try
        {
            var cachedValue = await GetAsync<T>(key);
            if (cachedValue != null)
            {
                return cachedValue;
            }

            var newValue = await factory();
            await SetAsync(key, newValue, expiry);
            return newValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in GetOrSetAsync for key: {key}");
            return await factory(); // Fallback to factory if cache fails
        }
    }
}
