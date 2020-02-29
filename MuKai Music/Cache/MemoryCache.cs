using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MuKai_Music.Cache
{
    public class MemCache : ICache
    {
        private readonly IMemoryCache _memoryCache;

        public CacheOption CacheOption { get; set; }

        public MemCache(IMemoryCache memoryCache)
        {
            this._memoryCache = memoryCache;
        }

        public bool Exists(string key)
            => this._memoryCache.TryGetValue(key, out object _);

        public async Task<bool> ExistsAsync(string key) =>
            await Task.FromResult(this._memoryCache.TryGetValue(key, out object _));

        public void DeleteKey(string key)
            => this._memoryCache.Remove(key);

        public string GetStringKey(string key)
            => this._memoryCache.Get(key) as string;

        public T GetStringKey<T>(string key)
            => JsonSerializer.Deserialize<T>(this._memoryCache.Get(key) as string);

        public void SetStringKey(string key, string value, TimeSpan expiry)
            => this._memoryCache.Set(key, value, expiry);

        public void SetStringKey(string key, object value, TimeSpan expiry)
            => this._memoryCache.Set(key, JsonSerializer.Serialize(value), expiry);

        public async Task DeleteKeyAsync(string key)
            => await Task.Run(() => this._memoryCache.Remove(key));

        public async Task<string> GetStringKeyAsync(string key)
            => await Task.Run(() => this._memoryCache.Get(key) as string);

        public async Task<T> GetStringKeyAsync<T>(string key)
            => await Task.Run(() => JsonSerializer.Deserialize<T>(this._memoryCache.Get(key) as string));

        public async Task SetStringKeyAsync(string key, string value, TimeSpan expiry)
            => await Task.Run(() => this._memoryCache.Set(key, value, expiry));

        public async Task SetStringKeyAsync(string key, object value, TimeSpan expiry)
            => await Task.Run(() => this._memoryCache.Set(key, JsonSerializer.Serialize(value), expiry));
    }
}
