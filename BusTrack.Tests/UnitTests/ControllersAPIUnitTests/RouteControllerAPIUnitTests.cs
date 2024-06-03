using Moq;
using Microsoft.AspNetCore.Mvc;
using BusTrack.BusTrack.API.ControllersAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.Tests.UnitTests.ControllersAPIUnitTests.TripControllerAPIUnitTests
{
    public class RouteControllerAPIUnitTests
    {
        private Mock<IRouteServiceAPI> _routeService;
        private RouteControllerAPI _controller;

        public RouteControllerAPIUnitTests()
        {
            _routeService = new Mock<IRouteServiceAPI>();
            _controller = new RouteControllerAPI(_routeService.Object);
        }

        [Fact]
        public void Get_ReturnsOkResult()
        {
            _routeService.Setup(service => service.GetRoutes()).Returns(Task.FromResult(new List<RouteDB>() {/* retornar uma lista de rotas */}));
            var result = _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

    }
}