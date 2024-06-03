using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
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
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Get all drivers");
        }

        [HttpGet("{id}", Name = "GetDriver")]
        public IActionResult GetById(int id)
        {
            return Ok($"Get driver with ID: {id}");
        }

        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            return Ok("Create new driver");
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] object value)
        {
            return Ok($"Update driver with ID: {id}");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"Delete driver with ID: {id}");
        }
    }
}
