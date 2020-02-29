using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace MuKai_Music.Cache
{
    /// <summary>
    /// 抽象对缓存的操作
    /// </summary>
    public interface ICache
    {
        void DeleteKey(string key);
        Task DeleteKeyAsync(string key);
        bool Exists(string key);
        Task<bool> ExistsAsync(string key);
        string GetStringKey(string key);
        T GetStringKey<T>(string key);
        Task<string> GetStringKeyAsync(string key);
        Task<T> GetStringKeyAsync<T>(string key);
        void SetStringKey(string key, string value, TimeSpan expiry);
        void SetStringKey(string key, object obj, TimeSpan expiry);
        Task SetStringKeyAsync(string key, string value, TimeSpan expiry);
        Task SetStringKeyAsync(string key, object value, TimeSpan expiry);
        CacheOption CacheOption { get; set; }

    }
}