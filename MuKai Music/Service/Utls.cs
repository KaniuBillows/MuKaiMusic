using Microsoft.Extensions.Configuration;

namespace MuKai_Music.Service
{
    public static class Utls
    {
        public static string GetCacheType(this IConfiguration configuration) => configuration["cache-type"];

        public static string GetCacheAge(this IConfiguration configuration) => configuration["cache-age"];

        public static string GetPivateKey(this IConfiguration configuration) => configuration["PrivateKey"];

        public static string GetExpires(this IConfiguration configuration) => configuration["Expires"];

        public static string GetSecurityKey(this IConfiguration configuration) => configuration["SecurityKey"];

        public static string GetDomain(this IConfiguration configuration) => configuration["Domain"];

        public static string GetRefreshTime(this IConfiguration configuration) => configuration["RefreshTime"];

        public static string GetPicRootPath(this IConfiguration configuration) => configuration["PicRoot"];

        public static string GetPicBaseUrl(this IConfiguration configuration) => configuration["PicBaseUrl"];

        public static string GetAccountAddress(this IConfiguration configuration) => configuration["AccountAddredss"];

        public static long GetMaxPicSize(this IConfiguration configuration) => long.Parse(configuration["MaxPicSize"]);
    }
}
