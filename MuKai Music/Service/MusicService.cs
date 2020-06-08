using DataAbstract;
using Microsoft.Extensions.Configuration;
using MuKai_Music.Service;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MuKai_Music.Model.Service
{
    public class MusicService : BaseRemoteService
    {

        public MusicService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {

        }

        public async Task<MusicInfo[]> SearchMusic(string key)
        {
            string route = "/search?keyword=" + key;
            Task<MusicInfo[]> neResult = ServiceRequest<MusicInfo[]>(DataSource.NetEase, route);
            Task<MusicInfo[]> kwResult = ServiceRequest<MusicInfo[]>(DataSource.Kuwo, route);
            Task<MusicInfo[]> miguResult = ServiceRequest<MusicInfo[]>(DataSource.Migu, route);
            MusicInfo[] neMusic = await neResult;
            MusicInfo[] kwMusic = await kwResult;
            MusicInfo[] miguMusic = await miguResult;
            MusicInfo[] res = neMusic.Concat(kwMusic).Concat(miguMusic).ToArray();
            res.Shuffle();
            return res;
        }

        public async Task<string> GetLyric(DataSource source, string id)
        {
            return await this.ServiceRequest(source, $"/lyric?id={id}");
        }

        public async Task<string> GetPic(string id, DataSource source)
        {
            return await ServiceRequest(source, "/pic?id=" + id);
        }

        public async Task<string> GetUrl(string id, DataSource source, string mid)
        {
            if (DataSource.Migu.Equals(source))
            {
                return await this.ServiceRequest(source, $"/url?cid={id}&id={mid}");
            }
            else
            {
                return await this.ServiceRequest(source, "/url?id=" + id);
            }
        }
    }
}
