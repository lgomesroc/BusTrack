using Moq;
using Microsoft.AspNetCore.Mvc;
using BusTrack.BusTrack.API.ControllersAPI;
using BusTrack.BusTrack.API.DTOAPI;
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
        public async Task Get_ReturnsOkWithPassengers()
        {
            var passengers = new[]
            {
                new PassengerDTOAPI
                {
                    Id = "1",
                    Name = "Maria",
                    Cpf = "12345678900",
                    Email = "maria@example.com",
                    Phone = "21999999999"
                }
            };

            _passengerService
                .Setup(service => service.GetAllPassengers())
                .ReturnsAsync(passengers);

            var result = await _controller.Get();

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            Assert.Same(passengers, okResult.Value);
        }

        [Fact]
        public async Task GetById_WhenPassengerExists_ReturnsOk()
        {
            var passenger = new PassengerDTOAPI
            {
                Id = "1",
                Name = "Maria",
                Cpf = "12345678900"
            };

            _passengerService
                .Setup(service => service.GetPassengerById("1"))
                .ReturnsAsync(passenger);

            var result = await _controller.GetById("1");

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            Assert.Same(passenger, okResult.Value);
        }

        [Fact]
        public async Task GetById_WhenPassengerDoesNotExist_ReturnsNotFound()
        {
            _passengerService
                .Setup(service => service.GetPassengerById("missing"))
                .ReturnsAsync((PassengerDTOAPI?)null);

            var result = await _controller.GetById("missing");

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
