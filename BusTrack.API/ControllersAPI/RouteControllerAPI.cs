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
            return Ok("Get all routes");
        }

        // GET: api/Route/5
        [HttpGet("{id}", Name = "GetRoute")]
        public IActionResult GetById(int id)
        {
            return Ok($"Get route with ID: {id}");
        }

        // POST: api/Route
        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            return Ok("Create new route");
        }

        // PUT: api/Route/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] object value)
        {
            return Ok($"Update route with ID: {id}");
        }

        // DELETE: api/Route/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"Delete route with ID: {id}");
        }
    }
}
