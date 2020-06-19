using Microsoft.AspNetCore.Mvc;

namespace Mukai_Playlist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {

        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}