using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverControllerAPI : ControllerBase
    {
        private readonly IDriverServiceAPI _driverService;

        public DriverControllerAPI(
            IDriverServiceAPI driverService)
        {
            _driverService = driverService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var drivers =
                await _driverService.GetAllDrivers();

            return Ok(drivers);
        }

        [HttpGet("{id}", Name = "GetDriver")]
        public async Task<IActionResult> GetById(
            string id)
        {
            var driver =
                await _driverService.GetDriverById(id);

            if (driver == null)
            {
                return NotFound();
            }

            return Ok(driver);
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody] DriverDTOAPI driver)
        {
            if (driver == null)
            {
                return BadRequest();
            }

            var createdDriver =
                await _driverService.CreateDriver(driver);

            return CreatedAtRoute(
                "GetDriver",
                new { id = createdDriver.Id },
                createdDriver);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(
            string id,
            [FromBody] DriverDTOAPI driver)
        {
            if (driver == null)
            {
                return BadRequest();
            }

            var updatedDriver =
                await _driverService.UpdateDriver(
                    id,
                    driver);

            if (updatedDriver == null)
            {
                return NotFound();
            }

            return Ok(updatedDriver);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            string id)
        {
            var deleted =
                await _driverService.DeleteDriver(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}