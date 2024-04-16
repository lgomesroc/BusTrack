using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusControllerAPI : ControllerBase
    {
        // GET: api/Bus
        [HttpGet]
        public IActionResult Get()
        {
            // Implementação para retornar a lista de ônibus
            return Ok("Get all buses");
        }

        // GET: api/Bus/5
        [HttpGet("{id}", Name = "GetBus")]
        public IActionResult GetById(int id)
        {
            // Implementação para retornar um ônibus pelo ID
            return Ok($"Get bus with ID: {id}");
        }

        // POST: api/Bus
        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            // Implementação para criar um novo ônibus
            return Ok("Create new bus");
        }

        // PUT: api/Bus/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] object value)
        {
            // Implementação para atualizar um ônibus existente
            return Ok($"Update bus with ID: {id}");
        }

        // DELETE: api/Bus/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Implementação para excluir um ônibus pelo ID
            return Ok($"Delete bus with ID: {id}");
        }
    }
}
