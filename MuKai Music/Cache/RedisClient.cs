using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;


namespace MuKai_Music.Cache
{
    public sealed class RedisClient : ICache
    {
        private readonly ConnectionMultiplexer redisMultiplexer;
        private readonly IDatabase db = null;

        public CacheOption CacheOption { get; set; }

        public RedisClient(IConfiguration Configuration)
        {
            try
            {
                string RedisConnection = Configuration.GetConnectionString("Redis");
                this.redisMultiplexer = ConnectionMultiplexer.Connect(RedisConnection);
                this.db = this.redisMultiplexer.GetDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                this.redisMultiplexer = null;
                this.db = null;
                Process.GetCurrentProcess().Kill();
            }
        }



        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        public async void SetStringKey(string key, string value, TimeSpan expiry) => await db.StringSetAsync(key, value, expiry);

        /// <summary>
        /// 保存key，value 不设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task SetStringKeyAsync(string key, string value) => await db.StringSetAsync(key, value);

        /// <summary>
        /// 保存key，value 不设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetStringKey(string key, string value) => db.StringSet(key, value);

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        public string GetStringKey(string key) => db.StringGet(key);

        /// <summary>
        /// 判断某个Key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key) => db.KeyExists(key);

        public async Task<bool> ExistsAsync(string key) => await db.KeyExistsAsync(key);

        /// <summary>
        /// 删除某个Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void DeleteKey(string key) => db.KeyDelete(key);

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        public T GetStringKey<T>(string key)
        {
            if (db == null)
            {
                return default;
            }
            RedisValue value = db.StringGet(key);
            return value.IsNullOrEmpty ? (default) : JsonSerializer.Deserialize<T>(value);
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        public void SetStringKey(string key, object obj, TimeSpan expiry)
        {
            if (db == null)
            {
                return;
            }
            string json = JsonSerializer.Serialize(obj);
            db.StringSetAsync(key, json, expiry);
        }

        public async Task DeleteKeyAsync(string key)
            => await db.KeyDeleteAsync(key);

        public async Task<string> GetStringKeyAsync(string key)
            => await db.StringGetAsync(key);

        public async Task<T> GetStringKeyAsync<T>(string key)
            => JsonSerializer.Deserialize<T>(await db.StringGetAsync(key));

        public async Task SetStringKeyAsync(string key, string value, TimeSpan expiry)
            => await db.StringSetAsync(key, value, expiry);

        public async Task SetStringKeyAsync(string key, object value, TimeSpan expiry)
            => await db.StringSetAsync(key, JsonSerializer.Serialize(value), expiry);
    }
}
