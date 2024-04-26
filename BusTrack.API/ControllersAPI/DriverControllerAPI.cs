using BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverControllerAPI : ControllerBase
    {
        private readonly IDriverServiceAPI _driveService;

        public DriverControllerAPI(IDriverServiceAPI driveService)
        {
            _driveService = driveService;
        }
        // GET: api/Driver
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Get all drivers");
        }

        // GET: api/Driver/5
        [HttpGet("{id}", Name = "GetDriver")]
        public IActionResult GetById(int id)
        {
            return Ok($"Get driver with ID: {id}");
        }

        // POST: api/Driver
        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            return Ok("Create new driver");
        }

        // PUT: api/Driver/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] object value)
        {
            return Ok($"Update driver with ID: {id}");
        }

        // DELETE: api/Driver/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"Delete driver with ID: {id}");
        }
    }
}
