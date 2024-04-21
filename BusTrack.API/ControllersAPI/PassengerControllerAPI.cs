using BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassengerControllerAPI : ControllerBase
    {
        private readonly IPassengerServiceAPI _passengerService;

        public PassengerControllerAPI(IPassengerServiceAPI passengerService)
        {
            _passengerService = passengerService;
        }
        // GET: api/Passenger
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Get all passengers");
        }

        // GET: api/Passenger/5
        [HttpGet("{id}", Name = "GetPassenger")]
        public IActionResult GetById(int id)
        {
            return Ok($"Get passenger with ID: {id}");
        }

        // POST: api/Passenger
        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            return Ok("Create new passenger");
        }

        // PUT: api/Passenger/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] object value)
        {
            return Ok($"Update passenger with ID: {id}");
        }

        // DELETE: api/Passenger/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"Delete passenger with ID: {id}");
        }
    }
}
