using MuKai_Music.Middleware;

namespace MuKai_Music.Cache
{
    /// <summary>
    /// 缓存设置，默认的缓存时长为86400秒，缓存方式为memoryCache
    /// </summary>
    public class CacheOption
    {
        public CacheOption()
        {
            Age = 86400;
            CacheType = CacheType.Memory;
        }

        public int Age { get; set; }
        public CacheType CacheType { get; set; }
    }
}
