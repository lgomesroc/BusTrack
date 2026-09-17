using BusTrack.BusTrack.API.ControllersAPI;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BusTrack.Tests.UnitTests.ControllersAPIUnitTests.TripsPassengerControllerAPIUnitTests
{
    public class TripsPassengerControllerAPIUnitTests
    {
        private readonly Mock<ITripsPassengerServiceAPI>
            _tripPassengerService;

        private readonly TripsPassengerControllerAPI
            _controller;

        public TripsPassengerControllerAPIUnitTests()
        {
            _tripPassengerService =
                new Mock<ITripsPassengerServiceAPI>();

            _controller =
                new TripsPassengerControllerAPI(
                    _tripPassengerService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult()
        {
            var tripPassengers =
                new List<TripPassengerDTOAPI>
                {
                    new TripPassengerDTOAPI
                    {
                        Id = "507f1f77bcf86cd799439011",
                        TripId = "507f1f77bcf86cd799439012",
                        PassengerId = "507f1f77bcf86cd799439013"
                    }
                };

            _tripPassengerService
                .Setup(service => service.GetAllAsync())
                .ReturnsAsync(tripPassengers);

            var result =
                await _controller.GetAll();

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            Assert.Equal(
                tripPassengers,
                okResult.Value);
        }

        [Fact]
        public async Task GetById_WhenExists_ReturnsOkResult()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            var tripPassenger =
                new TripPassengerDTOAPI
                {
                    Id = id,
                    TripId = "507f1f77bcf86cd799439012",
                    PassengerId = "507f1f77bcf86cd799439013"
                };

            _tripPassengerService
                .Setup(service =>
                    service.GetByIdAsync(id))
                .ReturnsAsync(tripPassenger);

            var result =
                await _controller.GetById(id);

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            Assert.Equal(
                tripPassenger,
                okResult.Value);
        }

        [Fact]
        public async Task GetById_WhenNotExists_ReturnsNotFound()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            _tripPassengerService
                .Setup(service =>
                    service.GetByIdAsync(id))
                .ReturnsAsync(
                    (TripPassengerDTOAPI?)null);

            var result =
                await _controller.GetById(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetByTripId_ReturnsOkResult()
        {
            const string tripId =
                "507f1f77bcf86cd799439012";

            var tripPassengers =
                new List<TripPassengerDTOAPI>
                {
                    new TripPassengerDTOAPI
                    {
                        Id = "507f1f77bcf86cd799439011",
                        TripId = tripId,
                        PassengerId = "507f1f77bcf86cd799439013"
                    }
                };

            _tripPassengerService
                .Setup(service =>
                    service.GetByTripIdAsync(tripId))
                .ReturnsAsync(tripPassengers);

            var result =
                await _controller.GetByTripId(tripId);

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            Assert.Equal(
                tripPassengers,
                okResult.Value);
        }

        [Fact]
        public async Task GetByPassengerId_ReturnsOkResult()
        {
            const string passengerId =
                "507f1f77bcf86cd799439013";

            var tripPassengers =
                new List<TripPassengerDTOAPI>
                {
                    new TripPassengerDTOAPI
                    {
                        Id = "507f1f77bcf86cd799439011",
                        TripId = "507f1f77bcf86cd799439012",
                        PassengerId = passengerId
                    }
                };

            _tripPassengerService
                .Setup(service =>
                    service.GetByPassengerIdAsync(
                        passengerId))
                .ReturnsAsync(tripPassengers);

            var result =
                await _controller.GetByPassengerId(
                    passengerId);

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            Assert.Equal(
                tripPassengers,
                okResult.Value);
        }

        [Fact]
        public async Task Create_WhenSuccessful_ReturnsCreatedAtRouteResult()
        {
            var tripPassenger =
                new TripPassengerDTOAPI
                {
                    TripId = "507f1f77bcf86cd799439012",
                    PassengerId = "507f1f77bcf86cd799439013"
                };

            var createdTripPassenger =
                new TripPassengerDTOAPI
                {
                    Id = "507f1f77bcf86cd799439011",
                    TripId = tripPassenger.TripId,
                    PassengerId = tripPassenger.PassengerId
                };

            _tripPassengerService
                .Setup(service =>
                    service.CreateAsync(tripPassenger))
                .ReturnsAsync(createdTripPassenger);

            var result =
                await _controller.Create(tripPassenger);

            var createdResult =
                Assert.IsType<CreatedAtRouteResult>(
                    result);

            Assert.Equal(
                "GetTripsPassenger",
                createdResult.RouteName);

            Assert.Equal(
                createdTripPassenger,
                createdResult.Value);
        }

        [Fact]
        public async Task Create_WhenBodyIsNull_ReturnsBadRequest()
        {
            var result =
                await _controller.Create(null!);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_WhenSuccessful_ReturnsOkResult()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            var tripPassenger =
                new TripPassengerDTOAPI
                {
                    Id = id,
                    TripId = "507f1f77bcf86cd799439012",
                    PassengerId = "507f1f77bcf86cd799439013"
                };

            _tripPassengerService
                .Setup(service =>
                    service.UpdateAsync(
                        id,
                        tripPassenger))
                .ReturnsAsync(tripPassenger);

            var result =
                await _controller.Update(
                    id,
                    tripPassenger);

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            Assert.Equal(
                tripPassenger,
                okResult.Value);
        }

        [Fact]
        public async Task Update_WhenBodyIsNull_ReturnsBadRequest()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            var result =
                await _controller.Update(
                    id,
                    null!);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_WhenTripPassengerDoesNotExist_ReturnsNotFound()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            var tripPassenger =
                new TripPassengerDTOAPI
                {
                    Id = id,
                    TripId = "507f1f77bcf86cd799439012",
                    PassengerId = "507f1f77bcf86cd799439013"
                };

            _tripPassengerService
                .Setup(service =>
                    service.UpdateAsync(
                        id,
                        tripPassenger))
                .ReturnsAsync(
                    (TripPassengerDTOAPI?)null);

            var result =
                await _controller.Update(
                    id,
                    tripPassenger);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_WhenSuccessful_ReturnsNoContent()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            _tripPassengerService
                .Setup(service =>
                    service.DeleteAsync(id))
                .ReturnsAsync(true);

            var result =
                await _controller.Delete(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_WhenNotExists_ReturnsNotFound()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            _tripPassengerService
                .Setup(service =>
                    service.DeleteAsync(id))
                .ReturnsAsync(false);

            var result =
                await _controller.Delete(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteByTripAndPassenger_WhenSuccessful_ReturnsNoContent()
        {
            const string tripId =
                "507f1f77bcf86cd799439012";

            const string passengerId =
                "507f1f77bcf86cd799439013";

            _tripPassengerService
                .Setup(service =>
                    service.DeleteByTripAndPassengerAsync(
                        tripId,
                        passengerId))
                .ReturnsAsync(true);

            var result =
                await _controller.DeleteByTripAndPassenger(
                    tripId,
                    passengerId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteByTripAndPassenger_WhenNotExists_ReturnsNotFound()
        {
            const string tripId =
                "507f1f77bcf86cd799439012";

            const string passengerId =
                "507f1f77bcf86cd799439013";

            _tripPassengerService
                .Setup(service =>
                    service.DeleteByTripAndPassengerAsync(
                        tripId,
                        passengerId))
                .ReturnsAsync(false);

            var result =
                await _controller.DeleteByTripAndPassenger(
                    tripId,
                    passengerId);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
