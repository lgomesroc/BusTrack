using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
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

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Get all trips");
        }

        [HttpGet("{id}", Name = "GetTrip")]
        public IActionResult GetById(int id)
        {
            return Ok($"Get trip with ID: {id}");
        }

        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            return Ok("Create new trip");
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] object value)
        {
            return Ok($"Update trip with ID: {id}");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"Delete trip with ID: {id}");
        }
    }
}
