
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/Maintenance")]
    [ApiController]
    [Authorize]
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
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}", Name = "GetMaintenance")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _service.GetByIdAsync(id);

            return item is null ? NotFound() : Ok(item);
        }

        [HttpGet("{id}/audit")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAudit(string id)
        {
            var history = await _service.GetAuditAsync(id);

            return history is null
                ? NotFound()
                : Ok(history);
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody] MaintenanceDTOAPI maintenance)
        {
            var created = await _service.CreateAsync(maintenance);

            if (created is null)
            {
                return BadRequest(new
                {
                    message = "Confira o ônibus informado, os campos obrigatórios, as datas, o custo e o status."
                });
            }

            return CreatedAtRoute(
                "GetMaintenance",
                new { id = created.Id },
                created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Put(
            string id,
            [FromBody] MaintenanceDTOAPI maintenance)
        {
            var updated = await _service.UpdateAsync(id, maintenance);

            if (updated is not null)
            {
                return Ok(updated);
            }

            var existing = await _service.GetByIdAsync(id);

            return existing is null
                ? NotFound()
                : BadRequest(new
                {
                    message = "Não foi possível atualizar. Confira os campos, as datas, o custo e se a manutenção permite alteração."
                });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(
            string id,
            [FromQuery] string? reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                return BadRequest(new
                {
                    message = "A justificativa da exclusão é obrigatória."
                });
            }

            var deleted = await _service.DeleteAsync(id, reason);

            if (deleted)
            {
                return NoContent();
            }

            var existing = await _service.GetByIdAsync(id);

            return existing is null
                ? NotFound()
                : BadRequest(new
                {
                    message = "A manutenção não pode ser excluída. Verifique o status e a justificativa."
                });
        }
    }
}
