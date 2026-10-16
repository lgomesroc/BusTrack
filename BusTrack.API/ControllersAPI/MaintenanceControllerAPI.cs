using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/Maintenance")]
    [ApiController]
    public class MaintenanceControllerAPI : ControllerBase
    {
        private readonly IMaintenanceServiceAPI _service;

        public MaintenanceControllerAPI(IMaintenanceServiceAPI service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}", Name = "GetMaintenance")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _service.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MaintenanceDTOAPI maintenance)
        {
            var created = await _service.CreateAsync(maintenance);
            if (created is null)
            {
                return BadRequest(new
                {
                    message = "Confira o ônibus informado, os campos obrigatórios, a data, o custo e o status."
                });
            }

            return CreatedAtRoute("GetMaintenance", new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] MaintenanceDTOAPI maintenance)
        {
            var updated = await _service.UpdateAsync(id, maintenance);
            if (updated is null)
            {
                var existing = await _service.GetByIdAsync(id);
                return existing is null
                    ? NotFound()
                    : BadRequest(new { message = "Dados de manutenção inválidos." });
            }

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return await _service.DeleteAsync(id) ? NoContent() : NotFound();
        }
    }
}