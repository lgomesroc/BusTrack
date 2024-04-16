using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripControllerAPI : ControllerBase
    {
        // GET: api/Trip
        [HttpGet]
        public IActionResult Get()
        {
            // Implementação para retornar a lista de viagens
            return Ok("Get all trips");
        }

        // GET: api/Trip/5
        [HttpGet("{id}", Name = "GetTrip")]
        public IActionResult GetById(int id)
        {
            // Implementação para retornar uma viagem pelo ID
            return Ok($"Get trip with ID: {id}");
        }

        // POST: api/Trip
        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            // Implementação para criar uma nova viagem
            return Ok("Create new trip");
        }

        // PUT: api/Trip/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] object value)
        {
            // Implementação para atualizar uma viagem existente
            return Ok($"Update trip with ID: {id}");
        }

        // DELETE: api/Trip/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Implementação para excluir uma viagem pelo ID
            return Ok($"Delete trip with ID: {id}");
        }
    }
}
