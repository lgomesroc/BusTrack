using System.Security.Claims;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/Trip")]
    [ApiController]
    public class TripControllerAPI : ControllerBase
    {
        private readonly ITripServiceAPI _tripService;

        public TripControllerAPI(ITripServiceAPI tripService) => _tripService = tripService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trips = await _tripService.GetAllTripDetailsAsync();
            return Ok(trips);
        }

        [Authorize]
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var trips = await _tripService.GetAllTripDetailsAsync();
            return Ok(trips.Where(trip => string.Equals(trip.UserId, userId, StringComparison.Ordinal)));
        }

        [HttpGet("{id}", Name = "GetTrip")]
        public async Task<IActionResult> GetById(string id)
        {
            var trip = await _tripService.GetTripDetailsByIdAsync(id);
            return trip == null ? NotFound() : Ok(trip);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TripDTOAPI trip)
        {
            if (trip == null) return BadRequest();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            trip.UserId = userId;
            var createdTrip = await _tripService.CreateTripAsync(trip);
            return CreatedAtRoute("GetTrip", new { id = createdTrip.Id }, createdTrip);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] TripDTOAPI trip)
        {
            if (trip == null) return BadRequest();
            var updatedTrip = await _tripService.UpdateTripAsync(id, trip);
            return updatedTrip == null ? NotFound() : Ok(updatedTrip);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _tripService.DeleteTripAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}