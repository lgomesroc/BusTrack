using Moq;
using Microsoft.AspNetCore.Mvc;
using BusTrack.BusTrack.API.ControllersAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.Tests.UnitTests.ControllersAPIUnitTests.DriverControllerAPIUnitTests
{
    public class DriverControllerAPIUnitTests
    {
        private Mock<IDriverServiceAPI> _driverService;
        private DriverControllerAPI _controller;

        public DriverControllerAPIUnitTests()
        {
            _driverService = new Mock<IDriverServiceAPI>();
            _controller = new DriverControllerAPI(_driverService.Object);
        }

        [Fact]
        public void Get_ReturnsOkResult()
        {
            _driverService.Setup(service => service.GetDrivers()).Returns(new List<DriverDB>() { /* lista de motoristas */ });
            var result = _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

    }
}