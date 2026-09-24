using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripControllerAPI : ControllerBase
    {
        private readonly ITripServiceAPI _tripService;

        public TripControllerAPI(
            ITripServiceAPI tripService)
        {
            _tripService = tripService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trips =
                await _tripService
                    .GetAllTripDetailsAsync();

            return Ok(trips);
        }

        [HttpGet("{id}", Name = "GetTrip")]
        public async Task<IActionResult> GetById(
            string id)
        {
            var trip =
                await _tripService
                    .GetTripDetailsByIdAsync(
                        id);

            if (trip == null)
            {
                return NotFound();
            }

            return Ok(trip);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] TripDTOAPI trip)
        {
            if (trip == null)
            {
                return BadRequest();
            }

            var createdTrip =
                await _tripService
                    .CreateTripAsync(
                        trip);

            return CreatedAtRoute(
                "GetTrip",
                new { id = createdTrip.Id },
                createdTrip);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            string id,
            [FromBody] TripDTOAPI trip)
        {
            if (trip == null)
            {
                return BadRequest();
            }

            var updatedTrip =
                await _tripService
                    .UpdateTripAsync(
                        id,
                        trip);

            if (updatedTrip == null)
            {
                return NotFound();
            }

            return Ok(updatedTrip);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            string id)
        {
            var deleted =
                await _tripService
                    .DeleteTripAsync(
                        id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
