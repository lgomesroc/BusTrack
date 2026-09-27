using BusTrack.BusTrack.API.ControllersAPI;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BusTrack.Tests.UnitTests.ControllersAPIUnitTests.TripControllerAPIUnitTests
{
    public class TripControllerAPIUnitTests
    {
        private readonly Mock<ITripServiceAPI> _tripService;
        private readonly TripControllerAPI _controller;

        public TripControllerAPIUnitTests()
        {
            _tripService =
                new Mock<ITripServiceAPI>();

            _controller =
                new TripControllerAPI(
                    _tripService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult()
        {
            var trips =
                new List<TripDetailsDTOAPI>
                {
                    new TripDetailsDTOAPI
                    {
                        Id = "507f1f77bcf86cd799439011",
                        Bus = new BusDetailsDTOAPI
                        {
                            Id = "507f1f77bcf86cd799439012",
                            Number = "001",
                            LicensePlate = "ABC1234",
                            Model = "Mercedes-Benz",
                            Capacity = 40
                        },
                        Driver = new DriverDetailsDTOAPI
                        {
                            Id = "507f1f77bcf86cd799439013",
                            Name = "John Doe",
                            LicenseNumber = "CNH123456"
                        },
                        Route = new RouteDetailsDTOAPI
                        {
                            Id = "507f1f77bcf86cd799439014",
                            Name = "Centro - Zona Sul",
                            Origin = "Centro",
                            Destination = "Zona Sul"
                        },
                        DepartureTime =
                            new DateTime(
                                2026,
                                9,
                                16,
                                16,
                                0,
                                0),
                        ArrivalTime =
                            new DateTime(
                                2026,
                                9,
                                16,
                                17,
                                0,
                                0),
                        Duration = 60,
                        LimitPassengers = 40,
                        Passengers = new List<PassengerDetailsDTOAPI>()
                    }
                };

            _tripService
                .Setup(service =>
                    service.GetAllTripDetailsAsync())
                .ReturnsAsync(trips);

            var result =
                await _controller.GetAll();

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            Assert.Equal(
                trips,
                okResult.Value);
        }

        [Fact]
        public async Task GetById_WhenExists_ReturnsOkResult()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            var trip =
                new TripDetailsDTOAPI
                {
                    Id = id,
                    Bus = new BusDetailsDTOAPI
                    {
                        Id = "507f1f77bcf86cd799439012",
                        Number = "001",
                        LicensePlate = "ABC1234",
                        Model = "Mercedes-Benz",
                        Capacity = 40
                    },
                    Driver = new DriverDetailsDTOAPI
                    {
                        Id = "507f1f77bcf86cd799439013",
                        Name = "John Doe",
                        LicenseNumber = "CNH123456"
                    },
                    Route = new RouteDetailsDTOAPI
                    {
                        Id = "507f1f77bcf86cd799439014",
                        Name = "Centro - Zona Sul",
                        Origin = "Centro",
                        Destination = "Zona Sul"
                    },
                    DepartureTime =
                        new DateTime(
                            2026,
                            9,
                            16,
                            16,
                            0,
                            0),
                    ArrivalTime =
                        new DateTime(
                            2026,
                            9,
                            16,
                            17,
                            0,
                            0),
                    Duration = 60,
                    LimitPassengers = 40,
                    Passengers = new List<PassengerDetailsDTOAPI>()
                };

            _tripService
                .Setup(service =>
                    service.GetTripDetailsByIdAsync(id))
                .ReturnsAsync(trip);

            var result =
                await _controller.GetById(id);

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            Assert.Equal(
                trip,
                okResult.Value);
        }

        [Fact]
        public async Task GetById_WhenNotExists_ReturnsNotFound()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            _tripService
                .Setup(service =>
                    service.GetTripDetailsByIdAsync(id))
                .ReturnsAsync(
                    (TripDetailsDTOAPI?)null);

            var result =
                await _controller.GetById(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_WhenSuccessful_ReturnsCreatedAtRouteResult()
        {
            var trip =
                new TripDTOAPI
                {
                    BusId = "507f1f77bcf86cd799439012",
                    DriverId = "507f1f77bcf86cd799439013",
                    RouteId = "507f1f77bcf86cd799439014",
                    DepartureTime =
                        new DateTime(
                            2026,
                            9,
                            16,
                            16,
                            0,
                            0)
                };

            var createdTrip =
                new TripDTOAPI
                {
                    Id = "507f1f77bcf86cd799439011",
                    BusId = trip.BusId,
                    DriverId = trip.DriverId,
                    RouteId = trip.RouteId,
                    DepartureTime =
                        trip.DepartureTime
                };

            _tripService
                .Setup(service =>
                    service.CreateTripAsync(trip))
                .ReturnsAsync(createdTrip);

            var result =
                await _controller.Create(trip);

            var createdResult =
                Assert.IsType<CreatedAtRouteResult>(
                    result);

            Assert.Equal(
                "GetTrip",
                createdResult.RouteName);

            Assert.Equal(
                createdTrip,
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

            var trip =
                new TripDTOAPI
                {
                    BusId = "507f1f77bcf86cd799439012",
                    DriverId = "507f1f77bcf86cd799439013",
                    RouteId = "507f1f77bcf86cd799439014"
                };

            var updatedTrip =
                new TripDTOAPI
                {
                    Id = id,
                    BusId = trip.BusId,
                    DriverId = trip.DriverId,
                    RouteId = trip.RouteId
                };

            _tripService
                .Setup(service =>
                    service.UpdateTripAsync(
                        id,
                        trip))
                .ReturnsAsync(updatedTrip);

            var result =
                await _controller.Update(
                    id,
                    trip);

            var okResult =
                Assert.IsType<OkObjectResult>(
                    result);

            Assert.Equal(
                updatedTrip,
                okResult.Value);
        }

        [Fact]
        public async Task Update_WhenTripDoesNotExist_ReturnsNotFound()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            var trip =
                new TripDTOAPI
                {
                    BusId = "507f1f77bcf86cd799439012"
                };

            _tripService
                .Setup(service =>
                    service.UpdateTripAsync(
                        id,
                        trip))
                .ReturnsAsync(
                    (TripDTOAPI?)null);

            var result =
                await _controller.Update(
                    id,
                    trip);

            Assert.IsType<NotFoundResult>(
                result);
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

            Assert.IsType<BadRequestResult>(
                result);
        }

        [Fact]
        public async Task Delete_WhenSuccessful_ReturnsNoContent()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            _tripService
                .Setup(service =>
                    service.DeleteTripAsync(id))
                .ReturnsAsync(true);

            var result =
                await _controller.Delete(id);

            Assert.IsType<NoContentResult>(
                result);
        }

        [Fact]
        public async Task Delete_WhenTripDoesNotExist_ReturnsNotFound()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            _tripService
                .Setup(service =>
                    service.DeleteTripAsync(id))
                .ReturnsAsync(false);

            var result =
                await _controller.Delete(id);

            Assert.IsType<NotFoundResult>(
                result);
        }
    }
}
