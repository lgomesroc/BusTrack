using System.Security.Claims;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/Trip")]
    [ApiController]
    public class TripControllerAPI : ControllerBase
    {
        private readonly ITripServiceAPI _tripService;
        private readonly IUserRepositoryDB _userRepository;
        private readonly IPassengerRepositoryDB _passengerRepository;

        public TripControllerAPI(
            ITripServiceAPI tripService,
            IUserRepositoryDB userRepository,
            IPassengerRepositoryDB passengerRepository)
        {
            _tripService = tripService;
            _userRepository = userRepository;
            _passengerRepository = passengerRepository;
        }

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

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var account = string.IsNullOrWhiteSpace(email) ? null : _userRepository.GetByEmail(email);
            var passengerIds = new HashSet<string>(StringComparer.Ordinal);

            if (account is not null)
            {
                var passengers = await _passengerRepository.GetAllPassengersAsync();
                foreach (var passenger in passengers)
                {
                    var emailMatches = !string.IsNullOrWhiteSpace(account.Email) &&
                        string.Equals(passenger.Email, account.Email, StringComparison.OrdinalIgnoreCase);
                    var cpfMatches = !string.IsNullOrWhiteSpace(account.CPF) &&
                        string.Equals(NormalizeCpf(passenger.Cpf), NormalizeCpf(account.CPF), StringComparison.Ordinal);
                    if ((emailMatches || cpfMatches) && !string.IsNullOrWhiteSpace(passenger.Id))
                        passengerIds.Add(passenger.Id);
                }
            }

            var trips = await _tripService.GetAllTripDetailsAsync();
            var mine = trips.Where(trip =>
                string.Equals(trip.UserId, userId, StringComparison.Ordinal) ||
                trip.Passengers.Any(passenger => !string.IsNullOrWhiteSpace(passenger.Id) && passengerIds.Contains(passenger.Id)));

            return Ok(mine);
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

        private static string NormalizeCpf(string? cpf) =>
            new string((cpf ?? string.Empty).Where(char.IsDigit).ToArray());
    }
}