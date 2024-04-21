using BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripControllerAPI : ControllerBase
    {
        private readonly ITripServiceAPI _tripService;

        public TripControllerAPI(ITripServiceAPI tripService)
        {
            _tripService = tripService;
        }
        // GET: api/Trip
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Get all trips");
        }

        // GET: api/Trip/5
        [HttpGet("{id}", Name = "GetTrip")]
        public IActionResult GetById(int id)
        {
            return Ok($"Get trip with ID: {id}");
        }

        // POST: api/Trip
        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            return Ok("Create new trip");
        }

        // PUT: api/Trip/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] object value)
        {
            return Ok($"Update trip with ID: {id}");
        }

        // DELETE: api/Trip/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"Delete trip with ID: {id}");
        }
    }
}
