using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusControllerAPI : ControllerBase
    {
        private readonly IBusServiceAPI _busService;

        public BusControllerAPI(
            IBusServiceAPI busService)
        {
            _busService = busService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var buses =
                await _busService.GetAllBuses();

            return Ok(buses);
        }

        [HttpGet("{id}", Name = "GetBus")]
        public async Task<IActionResult> GetById(
            string id)
        {
            var bus =
                await _busService.GetBusById(id);

            if (bus == null)
            {
                return NotFound();
            }

            return Ok(bus);
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody] BusDTOAPI bus)
        {
            if (bus == null)
            {
                return BadRequest();
            }

            var createdBus =
                await _busService.CreateBus(bus);

            return CreatedAtRoute(
                "GetBus",
                new { id = createdBus.Id },
                createdBus);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(
            string id,
            [FromBody] BusDTOAPI bus)
        {
            if (bus == null)
            {
                return BadRequest();
            }

            var updatedBus =
                await _busService.UpdateBus(
                    id,
                    bus);

            if (updatedBus == null)
            {
                return NotFound();
            }

            return Ok(updatedBus);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            string id)
        {
            var deleted =
                await _busService.DeleteBus(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
