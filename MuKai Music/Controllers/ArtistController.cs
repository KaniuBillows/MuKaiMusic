using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MuKai_Music.Controllers
{
    [Route("api/artist")]
    [ApiController]
    public class ArtistController : ControllerBase
    {

        /// <summary>
        /// 获取歌手介绍
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("description")]
        public async Task GetArtistDescription(int id) { }

        /// <summary>
        /// 获取歌手单曲
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("musics")]
        public async Task GetArtistMusics(int id) { }
    }
}
