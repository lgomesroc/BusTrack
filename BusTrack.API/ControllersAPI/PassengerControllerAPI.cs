using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/Passenger")]
    [ApiController]
    public class PassengerControllerAPI : ControllerBase
    {
        private readonly IPassengerServiceAPI _passengerService;

        public PassengerControllerAPI(
            IPassengerServiceAPI passengerService)
        {
            _passengerService = passengerService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var passengers =
                await _passengerService.GetAllPassengers();

            return Ok(passengers);
        }

        [HttpGet("{id}", Name = "GetPassenger")]
        public async Task<IActionResult> GetById(
            string id)
        {
            var passenger =
                await _passengerService.GetPassengerById(id);

            if (passenger == null)
            {
                return NotFound();
            }

            return Ok(passenger);
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody] PassengerDTOAPI passenger)
        {
            if (passenger == null)
            {
                return BadRequest();
            }

            var createdPassenger =
                await _passengerService.CreatePassenger(passenger);

            return CreatedAtRoute(
                "GetPassenger",
                new { id = createdPassenger.Id },
                createdPassenger);
        }
        [Authorize(Roles = "ADMIN")]

[HttpPut("{id}")]
        public async Task<IActionResult> Put(
            string id,
            [FromBody] PassengerDTOAPI passenger)
        {
            if (passenger == null)
            {
                return BadRequest();
            }

            var updatedPassenger =
                await _passengerService.UpdatePassenger(
                    id,
                    passenger);

            if (updatedPassenger == null)
            {
                return NotFound();
            }

            return Ok(updatedPassenger);
        }
        [Authorize(Roles = "ADMIN")]

[HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            string id)
        {
            var deleted =
                await _passengerService.DeletePassenger(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
