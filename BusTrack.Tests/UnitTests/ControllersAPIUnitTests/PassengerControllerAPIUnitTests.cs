using Moq;
using Microsoft.AspNetCore.Mvc;
using BusTrack.BusTrack.API.ControllersAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
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
            _passengerService.Setup(service => service.GetPassengers()).Returns(Task.FromResult(new List<PassengerDB>() {/* retornar uma lista de passageiros */}));

            var result = _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

    }
}