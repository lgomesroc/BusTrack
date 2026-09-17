using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsPassengerControllerAPI : ControllerBase
    {
        private readonly ITripsPassengerServiceAPI _tripPassengerService;

        public TripsPassengerControllerAPI(
            ITripsPassengerServiceAPI tripPassengerService)
        {
            _tripPassengerService = tripPassengerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tripPassengers =
                await _tripPassengerService.GetAllAsync();

            return Ok(tripPassengers);
        }

        [HttpGet("{id}", Name = "GetTripsPassenger")]
        public async Task<IActionResult> GetById(string id)
        {
            var tripPassenger =
                await _tripPassengerService.GetByIdAsync(id);

            if (tripPassenger == null)
            {
                return NotFound();
            }

            return Ok(tripPassenger);
        }

        [HttpGet("trip/{tripId}")]
        public async Task<IActionResult> GetByTripId(
            string tripId)
        {
            var tripPassengers =
                await _tripPassengerService.GetByTripIdAsync(
                    tripId);

            return Ok(tripPassengers);
        }

        [HttpGet("passenger/{passengerId}")]
        public async Task<IActionResult> GetByPassengerId(
            string passengerId)
        {
            var tripPassengers =
                await _tripPassengerService.GetByPassengerIdAsync(
                    passengerId);

            return Ok(tripPassengers);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] TripPassengerDTOAPI tripPassenger)
        {
            if (tripPassenger == null)
            {
                return BadRequest();
            }

            var createdTripPassenger =
                await _tripPassengerService.CreateAsync(
                    tripPassenger);

            return CreatedAtRoute(
                "GetTripsPassenger",
                new { id = createdTripPassenger.Id },
                createdTripPassenger);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            string id,
            [FromBody] TripPassengerDTOAPI tripPassenger)
        {
            if (tripPassenger == null)
            {
                return BadRequest();
            }

            var updatedTripPassenger =
                await _tripPassengerService.UpdateAsync(
                    id,
                    tripPassenger);

            if (updatedTripPassenger == null)
            {
                return NotFound();
            }

            return Ok(updatedTripPassenger);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted =
                await _tripPassengerService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("trip/{tripId}/passenger/{passengerId}")]
        public async Task<IActionResult> DeleteByTripAndPassenger(
            string tripId,
            string passengerId)
        {
            var deleted =
                await _tripPassengerService
                    .DeleteByTripAndPassengerAsync(
                        tripId,
                        passengerId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
