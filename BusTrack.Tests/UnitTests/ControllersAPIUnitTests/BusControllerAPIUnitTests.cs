using Moq;
using Microsoft.AspNetCore.Mvc;
using BusTrack.BusTrack.API.ControllersAPI;
using BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI;
using BusTrack.BusTrack.API.ServicesAPI;
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
            // Arrange
            _busService.Setup(service => service.GetBuses()).Returns(new List<BusDB>());

            // Act
            var result = _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        // Adicione mais testes para os outros métodos do controlador
    }
}