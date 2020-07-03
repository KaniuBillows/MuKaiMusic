using Microsoft.AspNetCore.Mvc;

namespace Mukai_Account.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        public HealthController()
        {

        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}