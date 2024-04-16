using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassengerControllerAPI : ControllerBase
    {
        // GET: api/Passenger
        [HttpGet]
        public IActionResult Get()
        {
            // Implementação para retornar a lista de passageiros
            return Ok("Get all passengers");
        }

        // GET: api/Passenger/5
        [HttpGet("{id}", Name = "GetPassenger")]
        public IActionResult GetById(int id)
        {
            // Implementação para retornar um passageiro pelo ID
            return Ok($"Get passenger with ID: {id}");
        }

        // POST: api/Passenger
        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            // Implementação para criar um novo passageiro
            return Ok("Create new passenger");
        }

        // PUT: api/Passenger/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] object value)
        {
            // Implementação para atualizar um passageiro existente
            return Ok($"Update passenger with ID: {id}");
        }

        // DELETE: api/Passenger/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Implementação para excluir um passageiro pelo ID
            return Ok($"Delete passenger with ID: {id}");
        }
    }
}
