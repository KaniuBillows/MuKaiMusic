using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DataAbstract;
using DataAbstract.Music;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mukai_Playlist.Filter;
using Mukai_Playlist.Service;

namespace Mukai_Playlist.Controllers
{
    [ApiController]
    [Route("")]
    public class PlaylistController : ControllerBase
    {
        private readonly PlaylistService playlistService;

        public PlaylistController(PlaylistService playlistService)
        {
            this.playlistService = playlistService;
        }
        /// <summary>
        /// 创建用户歌单，需要登录
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="loginUserId">由Filter自动参数注入</param>
        /// <returns></returns>
        [Authorize]
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
        [Authorize]
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
        [Authorization]
        [Authorize]
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
        /// <param name="id"></param>
        /// <returns></returns>
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
        [Authorization]
        [Authorize]
        [HttpPost("user/removeFromList")]
        public Task<Result> RemoveFromPlaylist(long loginUserId, [Required] string id, [Required][FromBody] List<string> sid)
        {
            return this.playlistService.RemoveMusicFromList(loginUserId, id, sid);
        }

    }
}
