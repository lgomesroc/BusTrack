using Moq;
using Microsoft.AspNetCore.Mvc;
using BusTrack.BusTrack.API.ControllersAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
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
            _tripService.Setup(service => service.GetTrip()).Returns(Task.FromResult(new List<TripDB>() {/* retornar uma lista de viagens */}));

            var result = _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

    }
}