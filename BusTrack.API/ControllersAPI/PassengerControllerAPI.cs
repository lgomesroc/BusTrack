using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
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

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Get all passengers");
        }

        [HttpGet("{id}", Name = "GetPassenger")]
        public IActionResult GetById(int id)
        {
            return Ok($"Get passenger with ID: {id}");
        }

        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            return Ok("Create new passenger");
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] object value)
        {
            return Ok($"Update passenger with ID: {id}");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"Delete passenger with ID: {id}");
        }
    }
}
