using BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{

    [Route("api/[controller]")]
    [ApiController]
    public class BusControllerAPI : ControllerBase
    {
        private readonly IBusServiceAPI _busService;

        public BusControllerAPI(IBusServiceAPI busService)
        {
            _busService = busService;
        }
        // GET: api/Bus
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Get all buses");
        }

        // GET: api/Bus/5
        [HttpGet("{id}", Name = "GetBus")]
        public IActionResult GetById(int id)
        {
            return Ok($"Get bus with ID: {id}");
        }

        // POST: api/Bus
        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            return Ok("Create new bus");
        }

        // PUT: api/Bus/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] object value)
        {
            return Ok($"Update bus with ID: {id}");
        }

        // DELETE: api/Bus/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"Delete bus with ID: {id}");
        }
    }
}
