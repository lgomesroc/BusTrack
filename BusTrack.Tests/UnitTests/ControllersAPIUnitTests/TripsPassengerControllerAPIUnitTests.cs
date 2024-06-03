using Moq;
using Microsoft.AspNetCore.Mvc;
using BusTrack.BusTrack.API.ControllersAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.Tests.UnitTests.ControllersAPIUnitTests.TripsPassengerControllerAPIUnitTests
{
    public class TripsPassengerControllerAPIUnitTests
    {
        private Mock<ITripsPassengerServiceAPI> _tripsPassengerService;
        private TripsPassengerControllerAPI _controller;

        public TripsPassengerControllerAPIUnitTests()
        {
            _tripsPassengerService = new Mock<ITripsPassengerServiceAPI>();
            _controller = new TripsPassengerControllerAPI(_tripsPassengerService.Object);
        }

        [Fact]
        public void Get_ReturnsOkResult()
        {
            _tripsPassengerService.Setup(service => service.GetTripsPassengers()).Returns(Task.FromResult(new List<TripPassengerDB>() {/* retornar uma lista de associações entre viagens e passageiros */}));
            var result = _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

    }
}
