using Moq;
using Microsoft.AspNetCore.Mvc;
using BusTrack.BusTrack.API.ControllersAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;

namespace BusTrack.Tests.UnitTests.ControllersAPIUnitTests.TripControllerAPIUnitTests
{
    public class PassengerControllerAPIUnitTests
    {
        private readonly Mock<IPassengerServiceAPI> _passengerService;
        private readonly PassengerControllerAPI _controller;

        public PassengerControllerAPIUnitTests()
        {
            _passengerService = new Mock<IPassengerServiceAPI>();
            _controller = new PassengerControllerAPI(
                _passengerService.Object);
        }

        [Fact]
        public void Get_ReturnsOkResult()
        {
            var result = _controller.Get();

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            Assert.Equal(
                "Get all passengers",
                okResult.Value);
        }
    }
}
