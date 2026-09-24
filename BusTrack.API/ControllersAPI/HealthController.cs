using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.Program.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("BusTrack API disponível.");
        }
    }
}
