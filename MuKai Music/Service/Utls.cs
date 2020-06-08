using DataAbstract;
using Microsoft.Extensions.Configuration;
using System;

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

        public static string GetBaseUrl(this IConfiguration configuration, DataSource source)
        {
            switch (source)
            {
                case DataSource.Kuwo:
                    {
                        return configuration["KuwoAPI"];
                    }

                case DataSource.NetEase:
                    {
                        return configuration["NeAPI"];
                    }
                case DataSource.Migu:
                    {
                        return configuration["MiguAPI"];
                    }
                default:
                    {
                        return "";
                    }
            }
        }

        /// <summary>
        /// 将数组乱序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            if (n == 0) return;
            Random random = new Random();
            for (int i = 0; i < array.Length; i++)
            {
                int rand = random.Next(i, n - 1);
                T temp = array[rand];
                array[rand] = array[i];
                array[i] = temp;
            }
        }
    }
}
