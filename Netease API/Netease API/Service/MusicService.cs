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
        public async Task<List<MusicInfo>> MuiscsProcess(List<MusicInfo> musics)
        {
            long[] ids = musics.Select(res => res.Ne_Id.Value).ToArray();
            Music_Url urlReq = new Music_Url(new Hashtable(), ids, 999000);
            MusicDetail detailReq = new MusicDetail(new Hashtable(), ids);
            Task<HttpResponseMessage> urlTask = urlReq.Request();
            Task<HttpResponseMessage> detailTask = detailReq.Request();
            HttpResponseMessage detail = await detailTask;
            SongDetailResult detail_Result = JsonSerializer.Deserialize<SongDetailResult>(await detail.Content.ReadAsStringAsync());
            HttpResponseMessage url = await urlTask;
            NetEaseUrl_Result url_Result = JsonSerializer.Deserialize<NetEaseUrl_Result>(await url.Content.ReadAsStringAsync());
            Dictionary<long, string> picHs = detail_Result.GetIdPicInfo();
            Dictionary<long, string> urlHs = url_Result.ToProcessedData();
            for (int i = 0; i < musics.Count; i++)
            {
                if (!urlHs.ContainsKey(musics[i].Ne_Id.Value))
                {
                    musics.RemoveAt(i);
                    continue;
                }
                else
                {
                    musics[i].Url = urlHs.GetValueOrDefault(musics[i].Ne_Id.Value, null);
                    musics[i].Album.PicUrl = picHs.GetValueOrDefault(musics[i].Ne_Id.Value, null);
                }
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
            long[] ids = musics.Select(mic => mic.Ne_Id.Value).ToArray();
            Music_Url urlReq = new Music_Url(new Hashtable(), ids, 999000);
            Task<HttpResponseMessage> urlTask = urlReq.Request();
            HttpResponseMessage url = await urlTask;
            NetEaseUrl_Result url_Result = JsonSerializer.Deserialize<NetEaseUrl_Result>(await url.Content.ReadAsStringAsync());
            Dictionary<long, string> urlHs = url_Result.ToProcessedData();
            for (int i = 0; i < musics.Count; i++)
            {
                if (!urlHs.ContainsKey(musics[i].Ne_Id.Value))
                {
                    musics.RemoveAt(i);
                    continue;
                }
                else
                {
                    musics[i].Url = urlHs.GetValueOrDefault(musics[i].Ne_Id.Value, null);
                }
            }
            return musics;
        }
    }
}
