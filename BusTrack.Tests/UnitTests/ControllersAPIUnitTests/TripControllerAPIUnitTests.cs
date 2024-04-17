using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using BusTrack.BusTrack.API.ControllersAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.Tests.UnitTests.ControllersAPIUnitTests.TripControllerAPIUnitTests
{
    public class TripControllerAPIUnitTests
    {
        private Mock<ITripServiceAPI> _tripService;
        private TripControllerAPI _controller;

        public TripControllerAPIUnitTests()
        {
            _tripService = new Mock<ITripServiceAPI>();
            _controller = new TripControllerAPI(_tripService.Object);
        }

        [Fact]
        public void Get_ReturnsOkResult()
        {
            // Arrange
            _tripService.Setup(service => service.GetTrip()).Returns(new List<TripDB>() {/* retornar uma lista de rotas */});

            // Act
            var result = _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        // Adicione mais testes para os outros métodos do controlador
    }
}