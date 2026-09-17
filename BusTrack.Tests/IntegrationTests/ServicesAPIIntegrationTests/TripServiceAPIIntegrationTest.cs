using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using BusTrack.Tests.MappingsIntegrationTests;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace BusTrack.Tests.IntegrationTests.ServicesAPIIntegrationTests
{
    public class TripServiceAPIIntegrationTest
    {
        private readonly Mock<ITripRepositoryDB>
            _tripRepository;

        private readonly IMapper _mapper;

        private readonly TripServiceAPI
            _tripServiceAPI;

        public TripServiceAPIIntegrationTest()
        {
            _tripRepository =
                new Mock<ITripRepositoryDB>();

            var config =
                new MapperConfiguration(
                    cfg =>
                    {
                        cfg.AddProfile<AutoMapperProfile>();
                    },
                    NullLoggerFactory.Instance);

            _mapper = config.CreateMapper();

            _tripServiceAPI =
                new TripServiceAPI(
                    _tripRepository.Object,
                    _mapper);
        }

        [Fact]
        public async Task GetAllTripsAsync_ReturnsAllTrips()
        {
            var trips =
                new List<TripDB>
                {
                    new TripDB
                    {
                        Id =
                            "507f1f77bcf86cd799439011",
                        BusId =
                            "507f1f77bcf86cd799439012",
                        DriverId =
                            "507f1f77bcf86cd799439013",
                        RouteId =
                            "507f1f77bcf86cd799439014"
                    },

                    new TripDB
                    {
                        Id =
                            "507f1f77bcf86cd799439015",
                        BusId =
                            "507f1f77bcf86cd799439016",
                        DriverId =
                            "507f1f77bcf86cd799439017",
                        RouteId =
                            "507f1f77bcf86cd799439018"
                    }
                };

            _tripRepository
                .Setup(repository =>
                    repository.GetAllTripsAsync())
                .ReturnsAsync(trips);

            var result =
                await _tripServiceAPI
                    .GetAllTripsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            _tripRepository.Verify(
                repository =>
                    repository.GetAllTripsAsync(),
                Times.Once);
        }

        [Fact]
        public async Task GetTripByIdAsync_WhenTripExists_ReturnsTrip()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            var trip =
                new TripDB
                {
                    Id = id,
                    BusId =
                        "507f1f77bcf86cd799439012",
                    DriverId =
                        "507f1f77bcf86cd799439013",
                    RouteId =
                        "507f1f77bcf86cd799439014"
                };

            _tripRepository
                .Setup(repository =>
                    repository.GetTripByIdAsync(id))
                .ReturnsAsync(trip);

            var result =
                await _tripServiceAPI
                    .GetTripByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result!.Id);

            Assert.Equal(
                trip.BusId,
                result.BusId);

            Assert.Equal(
                trip.DriverId,
                result.DriverId);

            Assert.Equal(
                trip.RouteId,
                result.RouteId);

            _tripRepository.Verify(
                repository =>
                    repository.GetTripByIdAsync(id),
                Times.Once);
        }

        [Fact]
        public async Task GetTripByIdAsync_WhenTripDoesNotExist_ReturnsNull()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            _tripRepository
                .Setup(repository =>
                    repository.GetTripByIdAsync(id))
                .ReturnsAsync(
                    (TripDB?)null);

            var result =
                await _tripServiceAPI
                    .GetTripByIdAsync(id);

            Assert.Null(result);

            _tripRepository.Verify(
                repository =>
                    repository.GetTripByIdAsync(id),
                Times.Once);
        }

        [Fact]
        public async Task CreateTripAsync_ReturnsCreatedTrip()
        {
            var trip =
                new TripDTOAPI
                {
                    BusId =
                        "507f1f77bcf86cd799439012",

                    DriverId =
                        "507f1f77bcf86cd799439013",

                    RouteId =
                        "507f1f77bcf86cd799439014",

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
                            18,
                            0,
                            0),

                    Duration = 120,

                    LimitPassengers = 40,

                    Passengers = new List<string>()
                };

            _tripRepository
                .Setup(repository =>
                    repository.AddTripAsync(
                        It.IsAny<TripDB>()))
                .ReturnsAsync(
                    (TripDB tripDB) =>
                    {
                        tripDB.Id =
                            "507f1f77bcf86cd799439011";

                        return tripDB;
                    });

            var result =
                await _tripServiceAPI
                    .CreateTripAsync(trip);

            Assert.NotNull(result);
            Assert.NotNull(result.Id);

            Assert.Equal(
                trip.BusId,
                result.BusId);

            Assert.Equal(
                trip.DriverId,
                result.DriverId);

            Assert.Equal(
                trip.RouteId,
                result.RouteId);

            Assert.Equal(
                trip.DepartureTime,
                result.DepartureTime);

            Assert.Equal(
                trip.ArrivalTime,
                result.ArrivalTime);

            Assert.Equal(
                trip.Duration,
                result.Duration);

            Assert.Equal(
                trip.LimitPassengers,
                result.LimitPassengers);

            _tripRepository.Verify(
                repository =>
                    repository.AddTripAsync(
                        It.IsAny<TripDB>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateTripAsync_WhenTripExists_ReturnsUpdatedTrip()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            var existingTrip =
                new TripDB
                {
                    Id = id,
                    BusId =
                        "507f1f77bcf86cd799439012",
                    DriverId =
                        "507f1f77bcf86cd799439013",
                    RouteId =
                        "507f1f77bcf86cd799439014"
                };

            var trip =
                new TripDTOAPI
                {
                    Id = id,
                    BusId =
                        "507f1f77bcf86cd799439015",

                    DriverId =
                        "507f1f77bcf86cd799439016",

                    RouteId =
                        "507f1f77bcf86cd799439017",

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
                            18,
                            0,
                            0),

                    Duration = 120,

                    LimitPassengers = 45,

                    Passengers = new List<string>()
                };

            _tripRepository
                .Setup(repository =>
                    repository.GetTripByIdAsync(id))
                .ReturnsAsync(existingTrip);

            _tripRepository
                .Setup(repository =>
                    repository.UpdateTripAsync(
                        id,
                        It.IsAny<TripDB>()))
                .ReturnsAsync(
                    (string _,
                        TripDB tripDB) =>
                    {
                        tripDB.Id = id;

                        return tripDB;
                    });

            var result =
                await _tripServiceAPI
                    .UpdateTripAsync(
                        id,
                        trip);

            Assert.NotNull(result);
            Assert.Equal(id, result!.Id);

            Assert.Equal(
                trip.BusId,
                result.BusId);

            Assert.Equal(
                trip.DriverId,
                result.DriverId);

            Assert.Equal(
                trip.RouteId,
                result.RouteId);

            Assert.Equal(
                trip.DepartureTime,
                result.DepartureTime);

            Assert.Equal(
                trip.ArrivalTime,
                result.ArrivalTime);

            Assert.Equal(
                trip.Duration,
                result.Duration);

            Assert.Equal(
                trip.LimitPassengers,
                result.LimitPassengers);

            _tripRepository.Verify(
                repository =>
                    repository.GetTripByIdAsync(id),
                Times.Once);

            _tripRepository.Verify(
                repository =>
                    repository.UpdateTripAsync(
                        id,
                        It.IsAny<TripDB>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateTripAsync_WhenTripDoesNotExist_ReturnsNull()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            var trip =
                new TripDTOAPI
                {
                    Id = id,
                    BusId =
                        "507f1f77bcf86cd799439012"
                };

            _tripRepository
                .Setup(repository =>
                    repository.GetTripByIdAsync(id))
                .ReturnsAsync(
                    (TripDB?)null);

            var result =
                await _tripServiceAPI
                    .UpdateTripAsync(
                        id,
                        trip);

            Assert.Null(result);

            _tripRepository.Verify(
                repository =>
                    repository.GetTripByIdAsync(id),
                Times.Once);

            _tripRepository.Verify(
                repository =>
                    repository.UpdateTripAsync(
                        It.IsAny<string>(),
                        It.IsAny<TripDB>()),
                Times.Never);
        }

        [Fact]
        public async Task DeleteTripAsync_WhenTripExists_ReturnsTrue()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            _tripRepository
                .Setup(repository =>
                    repository.DeleteTripAsync(id))
                .ReturnsAsync(true);

            var result =
                await _tripServiceAPI
                    .DeleteTripAsync(id);

            Assert.True(result);

            _tripRepository.Verify(
                repository =>
                    repository.DeleteTripAsync(id),
                Times.Once);
        }

        [Fact]
        public async Task DeleteTripAsync_WhenTripDoesNotExist_ReturnsFalse()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            _tripRepository
                .Setup(repository =>
                    repository.DeleteTripAsync(id))
                .ReturnsAsync(false);

            var result =
                await _tripServiceAPI
                    .DeleteTripAsync(id);

            Assert.False(result);

            _tripRepository.Verify(
                repository =>
                    repository.DeleteTripAsync(id),
                Times.Once);
        }
    }
}
