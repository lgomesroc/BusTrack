using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using BusTrack.BusTrack.API.ControllersAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.Tests.UnitTests.ControllersAPIUnitTests.TripControllerAPIUnitTests
{ 
    public class PassengerControllerAPIUnitTests
    {
        private Mock<IPassengerServiceAPI> _passengerService;
        private PassengerControllerAPI _controller;

        public PassengerControllerAPIUnitTests()
        {
            _passengerService = new Mock<IPassengerServiceAPI>();
            _controller = new PassengerControllerAPI(_passengerService.Object);
        }

        [Fact]
        public void Get_ReturnsOkResult()
        {
            // Arrange
            _passengerService.Setup(service => service.GetPassengers()).Returns(new List<PassengerDB>() {/* retornar uma lista de passageiros */});
            // Act
            var result = _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        // Adicione mais testes para os outros métodos do controlador
}
}