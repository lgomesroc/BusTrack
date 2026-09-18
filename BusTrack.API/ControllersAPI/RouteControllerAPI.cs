using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteControllerAPI : ControllerBase
    {
        private readonly IRouteServiceAPI _routeService;

        public RouteControllerAPI(
            IRouteServiceAPI routeService)
        {
            _routeService = routeService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var routes =
                await _routeService.GetAllRoutes();

            return Ok(routes);
        }

        [HttpGet("{id}", Name = "GetRoute")]
        public async Task<IActionResult> GetById(
            string id)
        {
            var route =
                await _routeService.GetRouteById(id);

            if (route == null)
            {
                return NotFound();
            }

            return Ok(route);
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody] RouteDTOAPI route)
        {
            if (route == null)
            {
                return BadRequest();
            }

            var createdRoute =
                await _routeService.CreateRoute(route);

            return CreatedAtRoute(
                "GetRoute",
                new { id = createdRoute.Id },
                createdRoute);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(
            string id,
            [FromBody] RouteDTOAPI route)
        {
            if (route == null)
            {
                return BadRequest();
            }

            var updatedRoute =
                await _routeService.UpdateRoute(
                    id,
                    route);

            if (updatedRoute == null)
            {
                return NotFound();
            }

            return Ok(updatedRoute);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            string id)
        {
            var deleted =
                await _routeService.DeleteRoute(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
