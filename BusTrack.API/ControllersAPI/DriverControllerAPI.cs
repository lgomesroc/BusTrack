using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverControllerAPI : ControllerBase
    {
        // GET: api/Driver
        [HttpGet]
        public IActionResult Get()
        {
            // Implementação para retornar a lista de motoristas
            return Ok("Get all drivers");
        }

        // GET: api/Driver/5
        [HttpGet("{id}", Name = "GetDriver")]
        public IActionResult GetById(int id)
        {
            // Implementação para retornar um motorista pelo ID
            return Ok($"Get driver with ID: {id}");
        }

        // POST: api/Driver
        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            // Implementação para criar um novo motorista
            return Ok("Create new driver");
        }

        // PUT: api/Driver/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] object value)
        {
            // Implementação para atualizar um motorista existente
            return Ok($"Update driver with ID: {id}");
        }

        // DELETE: api/Driver/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Implementação para excluir um motorista pelo ID
            return Ok($"Delete driver with ID: {id}");
        }
    }
}
