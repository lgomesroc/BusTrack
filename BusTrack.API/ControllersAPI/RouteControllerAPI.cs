using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
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

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Get all routes");
        }

        [HttpGet("{id}", Name = "GetRoute")]
        public IActionResult GetById(int id)
        {
            return Ok($"Get route with ID: {id}");
        }

        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            return Ok("Create new route");
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] object value)
        {
            return Ok($"Update route with ID: {id}");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"Delete route with ID: {id}");
        }
    }
}
