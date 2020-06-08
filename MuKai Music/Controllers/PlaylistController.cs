using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DataAbstract;
using DataAbstract.Music;
using Microsoft.AspNetCore.Mvc;
using MuKai_Music.Attribute;
using MuKai_Music.Extions.Util;
using MuKai_Music.Filter;
using MuKai_Music.Model.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MuKai_Music.Controllers
{
    [Route("api/playlist")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly PlaylistService playlistService;

        public PlaylistController(PlaylistService service)
        {
            this.playlistService = service;
        }

        /// <summary>
        /// 获取歌单详情
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("detail")]
        public async Task GetPlaylistDetail(long id)
        {
            await this.HttpContext.WirteBodyAsync(await this.playlistService.GetPlaylistDetail(id));
        }

        /// <summary>
        /// 创建用户歌单，需要登录
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="loginUserId">由Filter自动参数注入</param>
        /// <returns></returns>
#if !DEBUG
        [Authorize]
#endif
        [Authorization]
        [HttpPost("user/create")]
        public async Task<Result<UserPlaylist>> CreatePlaylist([FromQuery] long loginUserId, [FromBody] UserPlaylist playlist)
        {
            return await this.playlistService.CreateUserPlaylist(playlist, loginUserId);
        }

        /// <summary>
        /// 更新用户歌单，名字，封面，是否公开
        /// </summary>
        /// <param name="loginUserId"></param>
        /// <param name="id"></param>
        /// <param name="playlist"></param>
        /// <returns></returns>
#if !DEBUG
        [Authorize]
#endif
        [Authorization]
        [HttpPut("user/update")]
        public async Task<Result> UpdatePlaylist(long loginUserId,
                                                [Required] string id,
                                                [Required][FromBody] UserPlaylist playlist)
        {
            return await this.playlistService.UpdatePlaylist(loginUserId, id, playlist);
        }

        /// <summary>
        /// 获取用户创建的歌单列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        [HttpGet("user/list")]
        [ApiCache(NoStore = true)]
        [ResponseCache(NoStore = true)]
        public async Task<PageResult<UserPlaylist>> GetUserPlaylist([Required] long userId,
                                                                    int limit, int offset)
        {
            return await this.playlistService.GetUserPlaylist(userId, limit, offset);
        }

        /// <summary>
        /// 用户歌单详情信息,需要登录
        /// </summary>
        /// <param name="loginUserId">登录用户Id，由Filter注入</param>
        /// <param name="id"></param>
        /// <returns></returns>
#if !DEBUG
        [Authorize]
#endif
        [Authorization]
        [HttpGet("user/detail")]
        public async Task<Result<UserPlaylist>> GetUserPlaylistDetail(long loginUserId, [Required] string id)
        {
            var content = await this.playlistService.GetUserPlaylistDetail(loginUserId, id);
            return content != null ? Result<UserPlaylist>.SuccessReuslt(content)
                : Result<UserPlaylist>.FailResult("歌单不存在或者您没有访问权限", 403);
        }

        /// <summary>
        /// 删除歌单
        /// </summary>
        /// <param name="loginUserId"></param>
        /// <param name="listId"></param>
        /// <returns></returns>
#if !DEBUG
        [Authorize]
#endif
        [Authorization]
        [HttpDelete("user/delete")]
        public Task<Result> DeletePlaylist(long loginUserId, [Required] string id)
        {
            return this.playlistService.DeletePlaylist(loginUserId, id);
        }

        /// <summary>
        /// 向歌单中加入歌曲
        /// </summary>
        /// <param name="loginUserId"></param>
        /// <param name="id"></param>
        /// <param name="musicInfos"></param>
        /// <returns></returns>
#if !DEBUG
        [Authorize]
#endif
        [Authorization]
        [HttpPost("user/insertToList")]
        public Task<Result> InsertToPlaylist(long loginUserId,
                                            [Required] string id,
                                            [Required][FromBody] List<MusicInfo> musicInfos)
        {
            return this.playlistService.AddMusicToPlaylist(loginUserId, id, musicInfos);
        }

        /// <summary>
        /// 从歌单中移除某些歌曲
        /// </summary>
        /// <param name="loginUserId"></param>
        /// <param name="id"></param>
        /// <param name="sid">Sid，(int)DataSource_(string)Id格式,咪咕的Id为CopyrightId</param>
        /// <returns></returns>
#if !DEBUG
        [Authorize]
#endif
        [Authorization]
        [HttpPost("user/removeFromList")]
        public Task<Result> RemoveFromPlaylist(long loginUserId, [Required] string id, [Required][FromBody] List<string> sid)
        {
            return this.playlistService.RemoveMusicFromList(loginUserId, id, sid);
        }


        /// <summary>
        /// 网易云推荐歌单
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("personlized")]
        public async Task PersonalizedPlaylist(int limit = 10)
        {
            await this.HttpContext.WirteBodyAsync(await this.playlistService.GetPersonalizedPlaylist(limit));
        }



        /// <summary>
        /// 获取相似歌单
        /// </summary>
        /// <param name="id">歌曲Id</param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        [HttpGet("similar")]
        public async Task GetSimilarPlaylist(int id, int limit, int offset) { }

        /// <summary>
        /// 获取分类下的歌单
        /// </summary>
        /// <param name="category"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        [HttpGet("underCategory")]
        public async Task GetPlaylistInCategory(string category, int limit, int offset) { }

        /// <summary>
        /// 获取精品歌单
        /// </summary>
        /// <param name="category"></param>
        /// <param name="limit"></param>
        [HttpGet("highQuality")]
        public async Task HighQualityPlaylist(string category, int limit) { }


        /// <summary>
        /// 获取全部歌单分类
        /// </summary>
        [HttpGet("allCategories")]
        [ResponseCache(CacheProfileName = "longTime")]
        public async Task GetCategories() { }

        /// <summary>
        /// 获取热门歌单分类
        /// </summary>
        [HttpGet("hotCategories")]
        [ResponseCache(CacheProfileName = "longTime")]
        public async Task GetHotCategories() { }


    }
}
