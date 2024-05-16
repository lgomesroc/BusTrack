using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using BusTrack.BusTrack.DB.ModelsDB;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardControllerAPI : ControllerBase
    {
        private readonly ITripRepositoryDB _tripRepository;

        public DashboardControllerAPI(ITripRepositoryDB tripRepository)
        {
            _tripRepository = tripRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTrips()
        {
            try
            {
                var trips = await _tripRepository.GetAllTripsAsync();
                return Ok(trips);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripById(string id)
        {
            try
            {
                var trip = await _tripRepository.GetTripByIdAsync(id);
                if (trip == null)
                {
                    return NotFound();
                }
                return Ok(trip);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddTrip([FromBody] TripDB trip)
        {
            if (trip == null)
            {
                return BadRequest("Dados inválidos");
            }

            try
            {
                await _tripRepository.AddTripAsync(trip);
                return CreatedAtAction(nameof(GetTripById), new { id = trip.Id }, trip);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrip(string id, [FromBody] TripDB trip)
        {
            if (trip == null || trip.Id != id)
            {
                return BadRequest("Dados inválidos");
            }

            try
            {
                await _tripRepository.UpdateTripAsync(id, trip);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(string id)
        {
            try
            {
                var isDeleted = await _tripRepository.DeleteTripAsync(id);
                if (!isDeleted)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}
