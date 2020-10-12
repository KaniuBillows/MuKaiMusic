using DataAbstract;
using MusicApi.NetEase.Music;
using Netease_API.Results;
using Netease_API.Results.Music;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Netease_API.Service
{
    public class MusicService
    {
        /// <summary>
        /// 获取音乐图片，排除无法获取URL的歌曲
        /// </summary>
        /// <param name="musics"></param>
        /// <returns></returns>
        public async Task<List<MusicInfo>> MusicsProcess(List<MusicInfo> musics)
        {
            var ids = musics.Select(res => res.Ne_Id.Value).ToArray();
            Music_Url urlReq = new Music_Url(new Hashtable(), ids, 999000);
            MusicDetail detailReq = new MusicDetail(new Hashtable(), ids);
            var urlTask = urlReq.Request();
            var detailTask = detailReq.Request();
            HttpResponseMessage detail = await detailTask;
            SongDetailResult detailResult =
                JsonSerializer.Deserialize<SongDetailResult>(await detail.Content.ReadAsStringAsync());
            HttpResponseMessage url = await urlTask;
            NetEaseUrl_Result urlResult =
                JsonSerializer.Deserialize<NetEaseUrl_Result>(await url.Content.ReadAsStringAsync());
            var picHs = detailResult.GetIdPicInfo();
            var urlHs = urlResult.ToProcessedData();

            for (var i = musics.Count - 1; i >= 0; i--)
            {
                var neId = musics[i].Ne_Id;
                urlHs.TryGetValue(neId.Value, out var value);
                if (value != null)
                {
                    musics[i].Url = value;
                    musics[i].Album.PicUrl = picHs.GetValueOrDefault(musics[i].Ne_Id.Value, null);
                }
                else
                    musics.RemoveAt(i);
            }

            return musics;
        }

        /// <summary>
        /// 排除无法获取URL的歌曲
        /// </summary>
        /// <param name="musics"></param>
        /// <returns></returns>
        public async Task<List<MusicInfo>> FilterUrl(List<MusicInfo> musics)
        {
            var ids = musics.Select(mic => mic.Ne_Id.Value).ToArray();
            Music_Url urlReq = new Music_Url(new Hashtable(), ids, 999000);
            var urlTask = urlReq.Request();
            HttpResponseMessage url = await urlTask;
            NetEaseUrl_Result urlResult =
                JsonSerializer.Deserialize<NetEaseUrl_Result>(await url.Content.ReadAsStringAsync());
            var urlHs = urlResult.ToProcessedData();
            for (var i = musics.Count - 1; i >= 0; i--)
            {
                var neId = musics[i].Ne_Id;
                urlHs.TryGetValue(neId.Value, out var value);
                if (value != null)
                {
                    musics[i].Url = value;
                }
                else
                    musics.RemoveAt(i);
            }

            return musics;
        }
    }
}