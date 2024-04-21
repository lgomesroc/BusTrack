using BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsPassengerControllerAPI : ControllerBase
    {
        private readonly ITripsPassengerServiceAPI _trippassengerService;

        public TripsPassengerControllerAPI(ITripsPassengerServiceAPI trippassengerService)
        {
            _trippassengerService = trippassengerService;
        }
        // GET: api/TripsPassenger
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Get all trips passengers");
        }

        // GET: api/TripsPassenger/5
        [HttpGet("{id}", Name = "GetTripsPassenger")]
        public IActionResult GetById(int id)
        {
            return Ok($"Get trip passenger with ID: {id}");
        }

        // POST: api/TripsPassenger
        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            return Ok("Create new trip passenger");
        }

        // PUT: api/TripsPassenger/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] object value)
        {
            return Ok($"Update trip passenger with ID: {id}");
        }

        // DELETE: api/TripsPassenger/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"Delete trip passenger with ID: {id}");
        }
    }
}
