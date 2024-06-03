using Moq;
using Microsoft.AspNetCore.Mvc;
using BusTrack.BusTrack.API.ControllersAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.Tests.UnitTests.ControllersAPIUnitTests.BusControllerAPIUnitTests
{
    public class BusControllerAPIUnitTests
    {
        private Mock<IBusServiceAPI> _busService;
        private BusControllerAPI _controller;

        public BusControllerAPIUnitTests()
        {
            _busService = new Mock<IBusServiceAPI>();
            _controller = new BusControllerAPI(_busService.Object);
        }

        [Fact]
        public void Get_ReturnsOkResult()
        {
            _busService.Setup(service => service.GetBuses()).Returns(new List<BusDB>());

            var result = _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

    }
}