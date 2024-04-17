using BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteControllerAPI : ControllerBase
    {
        private readonly IRouteServiceAPI _routeService;

        public RouteControllerAPI(IRouteServiceAPI routeService)
        {
            _routeService = routeService;
        }
        // GET: api/Route
        [HttpGet]
        public IActionResult Get()
        {
            // Implementação para retornar a lista de rotas
            return Ok("Get all routes");
        }

        // GET: api/Route/5
        [HttpGet("{id}", Name = "GetRoute")]
        public IActionResult GetById(int id)
        {
            // Implementação para retornar uma rota pelo ID
            return Ok($"Get route with ID: {id}");
        }

        // POST: api/Route
        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            // Implementação para criar uma nova rota
            return Ok("Create new route");
        }

        // PUT: api/Route/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] object value)
        {
            // Implementação para atualizar uma rota existente
            return Ok($"Update route with ID: {id}");
        }

        // DELETE: api/Route/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Implementação para excluir uma rota pelo ID
            return Ok($"Delete route with ID: {id}");
        }
    }
}
