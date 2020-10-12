using System;
using System.Collections;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DataAbstract;
using DataAbstract.Playlist;
using Microsoft.AspNetCore.Mvc;
using MusicApi.NetEase.Music;
using MusicApi.NetEase.Playlist;
using Netease_API.Results;
using Netease_API.Results.Music;
using Netease_API.Results.Playlist;
using Netease_API.Service;

namespace Netease_API.Controllers
{
    [ApiController]
    [Route("playlist")]
    public class PlaylistController : ControllerBase
    {
        private readonly MusicService musicService;

        public PlaylistController(MusicService musicService)
        {
            this.musicService = musicService;
        }

        /// <summary>
        /// 获取歌单详情，包含每首歌的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("detail")]
        public async Task<Result<PlaylistInfo>> PlaylistDetail(long id)
        {
            PlaylistDetail request = new PlaylistDetail(id);
            try
            {
                HttpResponseMessage response = await request.Request();
                PlaylistDetailResult res =
                    JsonSerializer.Deserialize<PlaylistDetailResult>(await response.Content.ReadAsStringAsync());
                if (res.Code != 200) return Result<PlaylistInfo>.FailResult("服务不可用");
                var ids = res.Playlist.SongIds.Select(i => i.Id).ToArray();
                MusicDetail musicDetailReq = new MusicDetail(ids);
                var detailTask = musicDetailReq.Request();
                SongDetailResult detailResult =
                    JsonSerializer.Deserialize<SongDetailResult>(await (await detailTask).Content.ReadAsStringAsync());
                return Result<PlaylistInfo>.SuccessReuslt(new PlaylistInfo()
                {
                    DataSource = DataSource.NetEase,
                    Id = res.Playlist.Id,
                    MusicCount = res.Playlist.SongCount,
                    Musics = await musicService.MusicsProcess(detailResult.ToProcessedData()),
                    Name = res.Playlist.Name,
                    PicUrl = res.Playlist.PicUrl
                });
            }
            catch (Exception)
            {
                return Result<PlaylistInfo>.FailResult("服务不可用");
            }
        }

        /// <summary>
        /// 网易云推荐歌单
        /// </summary>
        /// <param name="limit">数量,默认5</param>
        /// <returns></returns>
        [HttpGet("personalized")]
        public async Task<Result<PlaylistInfo[]>> PersonalizedPlaylist(int limit = 5)
        {
            PlaylistPersonalized request = new PlaylistPersonalized(limit);
            HttpResponseMessage response = await request.Request();
            try
            {
                PersonlizedPlaylistResult result =
                    JsonSerializer.Deserialize<PersonlizedPlaylistResult>(await response.Content.ReadAsStringAsync());
                return Result<PlaylistInfo[]>.SuccessReuslt(result.ToProcessedData());
            }
            catch (Exception)
            {
                return Result<PlaylistInfo[]>.FailResult("获取歌单异常");
            }
        }
    }
}