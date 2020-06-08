using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DataAbstract;
using Microsoft.Extensions.Configuration;
using MuKai_Music.Service;

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
            return res;
        }

        public Task<string> GetLyric(DataSource source, string id)
        {
            return this.ServiceRequest(source, $"/lyric?id={id}");
        }

        public Task<string> GetPic(string id, DataSource source)
        {
            return ServiceRequest(source, $"/pic?id={id}");
        }

        public Task<string> GetUrl(string id, DataSource source)
        {
            return this.ServiceRequest(source, $"/url?id={id}");
        }
    }
}
